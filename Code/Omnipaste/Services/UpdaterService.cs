namespace Omnipaste.Services
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reactive.Linq;
    using System.Reflection;
    using NAppUpdate.Framework;
    using NAppUpdate.Framework.Sources;
    using NAppUpdate.Framework.Tasks;
    using OmniCommon;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Framework;

    public class UpdaterService : IUpdaterService
    {
        private const string InstallerName = "OmnipasteInstaller.msi";

        private const string UpdateFeedFileName = "FeedBuilder.xml";

        private const string MSIExec = "msiexec.exe";

        private readonly TimeSpan _initialUpdateCheckDelay = TimeSpan.FromSeconds(15);
        private readonly TimeSpan _updateCheckInterval = TimeSpan.FromMinutes(60);
        private readonly TimeSpan _systemIdleThreshold = TimeSpan.FromMinutes(5);

        private readonly UpdateManager _updateManager;

        private IDisposable _systemIdleObserver;

        public ISystemIdleService SystemIdleService { get; set; }

        protected static string RootDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);

                return Path.GetDirectoryName(path);
            }
        }

        protected static string FeedUrl
        {
            get
            {
                return string.Format(
                    "{0}{1}",
                    ConfigurationManager.AppSettings[ConfigurationProperties.UpdateSource],
                    UpdateFeedFileName);
            }
        }

        protected static string AppName
        {
            get
            {
                return ConfigurationManager.AppSettings[ConfigurationProperties.AppName];
            }
        }

        protected static string InstallerTemporaryFolder
        {
            get
            {
                return Path.Combine(Path.GetTempPath(), AppName);
            }
        }

        protected static string MsiTemporaryPath
        {
            get
            {
                return Path.Combine(InstallerTemporaryFolder, InstallerName);
            }
        }

        public UpdaterService(ISystemIdleService systemIdleService)
        {
            SystemIdleService = systemIdleService;
            _updateManager = UpdateManager.Instance;
            _updateManager.UpdateSource = new SimpleWebSource(FeedUrl) { Proxy = WebRequest.GetSystemWebProxy() };
            _updateManager.ReinstateIfRestarted();
        }

        public IObservable<bool> CreateUpdateAvailableObservable(TimeSpan updateCheckInterval)
        {
            var timer = Observable.Timer(_initialUpdateCheckDelay, updateCheckInterval);
            return timer.Select(_ => _updateManager.AreUpdatesAvailable().Select(__ => NewInstallerAvailable())).Switch();
        }

        public IObservable<bool> DownloadUpdates()
        {
            return _updateManager.DownloadUpdates().Select(
                couldDownloadUpdates =>
                {
                    if (couldDownloadUpdates) PrepareDownloadedInstaller();
                    return couldDownloadUpdates;
                });
        }

        public void SetupAutoUpdate(TimeSpan? updateCheckInterval = null, TimeSpan? systemIdleThreshold = null)
        {
            updateCheckInterval = updateCheckInterval ?? _updateCheckInterval;
            systemIdleThreshold = systemIdleThreshold ?? _systemIdleThreshold;
            CreateUpdateAvailableObservable(updateCheckInterval.Value)
                .Where(updateAvailable => updateAvailable)
                .Select(_ => DownloadUpdates())
                .Switch()
                .CatchAndReport()
                .Where(couldDownloadUpdates => couldDownloadUpdates)
                .ObserveOn(SchedulerProvider.Dispatcher)
                .Subscribe(_ => InstallNewVersionWhenIdle(systemIdleThreshold.Value));
        }

        public void InstallNewVersionWhenIdle(TimeSpan systemIdleThreshold)
        {
            DisposeSystemIdleObserver();
            _systemIdleObserver =
                SystemIdleService.CreateSystemIdleObservable(systemIdleThreshold)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .Where(systemIsIdle => systemIsIdle)
                    .Subscribe(
                        _ =>
                            {
                                DisposeSystemIdleObserver();
                                InstallNewVersion();
                            });
        }

        public void InstallNewVersion()
        {
            Process.Start(MSIExec, string.Format("/i {0} /qn", MsiTemporaryPath));
        }

        public void CleanTemporaryFiles()
        {
            if (Directory.Exists(InstallerTemporaryFolder))
            {
                Directory.Delete(InstallerTemporaryFolder, true);
            }
        }

        private static bool RemoteInstallerHasHigherVersion(FileUpdateTask installerUpdateTask)
        {
            Version msiVersion;
            Version.TryParse(installerUpdateTask.Version, out msiVersion);
            var exeVersion = Assembly.GetEntryAssembly().GetName().Version;

            return msiVersion > exeVersion;
        }

        private bool NewInstallerAvailable()
        {
            var updateInstallerTask = GetUpdateInstallerTask();

            return updateInstallerTask != null && RemoteInstallerHasHigherVersion(updateInstallerTask);
        }

        private void PrepareDownloadedInstaller()
        {
            CleanTemporaryFiles();
            var updateInstallerTask = GetUpdateInstallerTask();
            _updateManager.ApplyUpdates(false);
            Directory.CreateDirectory(InstallerTemporaryFolder);
            //Move new msi to a temp file as the app directory might get uninstalled
            var installerPath = Path.Combine(RootDirectory, updateInstallerTask.LocalPath);
            File.Copy(installerPath, MsiTemporaryPath);
        }

        private FileUpdateTask GetUpdateInstallerTask()
        {
            return
                _updateManager.Tasks.Where(task => task is FileUpdateTask)
                    .Cast<FileUpdateTask>()
                    .FirstOrDefault(fileUpdateTask => fileUpdateTask.LocalPath == InstallerName);
        }

        private void DisposeSystemIdleObserver()
        {
            if (_systemIdleObserver != null)
            {
                _systemIdleObserver.Dispose();
            }
        }
    }
}