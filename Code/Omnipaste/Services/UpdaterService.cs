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
    using BugFreak;
    using Microsoft.Deployment.WindowsInstaller;
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

        public void SetupAutoUpdate(TimeSpan? updateCheckInterval = null, TimeSpan? systemIdleThreshold = null)
        {
            updateCheckInterval = updateCheckInterval ?? _updateCheckInterval;
            systemIdleThreshold = systemIdleThreshold ?? _systemIdleThreshold;
            AreUpdatesAvailable(updateCheckInterval.Value)
                .Where(updateAvailable => updateAvailable)
                .Select(_ => DownloadUpdates())
                .Switch()
                .CatchAndReport()
                .Where(couldDownloadUpdates => couldDownloadUpdates)
                .ObserveOn(SchedulerProvider.Dispatcher)
                .Subscribe(_ => InstallNewVersionWhenIdle(systemIdleThreshold.Value));
        }

        public IObservable<bool> AreUpdatesAvailable(TimeSpan updateCheckInterval)
        {
            var timer = Observable.Timer(_initialUpdateCheckDelay, updateCheckInterval);
            return timer.Select(_ => _updateManager.AreUpdatesAvailable(NewRemoteInstallerAvailable)).Switch();
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
            try
            {
                Process.Start(MSIExec, string.Format("/i {0} /qn", MsiTemporaryPath));
            }
            catch (Exception exception)
            {
                ReportingService.Instance.BeginReport(exception);
            }
        }

        public void CleanTemporaryFiles()
        {
            if (Directory.Exists(InstallerTemporaryFolder))
            {
                Directory.Delete(InstallerTemporaryFolder, true);
            }
        }

        public bool NewLocalInstallerAvailable()
        {
            bool result;
            try
            {
                result = File.Exists(MsiTemporaryPath) && MsiHasHigherVersion(MsiTemporaryPath);
            }
            catch (Exception exception)
            {
                result = false;
                ReportingService.Instance.BeginReport(exception);
            }

            return result;
        }

        private static bool RemoteInstallerHasHigherVersion(FileUpdateTask installerUpdateTask)
        {
            return VersionIsHigherThanOwn(installerUpdateTask.Version);
        }

        private static bool MsiHasHigherVersion(string msiPath)
        {
            return VersionIsHigherThanOwn(GetMsiVersion(msiPath));
        }

        private static bool VersionIsHigherThanOwn(string versionString)
        {
            Version msiVersion;
            Version.TryParse(versionString, out msiVersion);
            var exeVersion = Assembly.GetEntryAssembly().GetName().Version;

            return msiVersion > exeVersion;
        }

        private static string GetMsiVersion(string msiPath)
        {
            string versionString;
            using (var database = new Database(msiPath))
            {
                versionString = database.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = '{0}'", "ProductVersion") as string;
            }

            return versionString;
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

        private bool NewRemoteInstallerAvailable()
        {
            var updateInstallerTask = GetUpdateInstallerTask();

            return updateInstallerTask != null && RemoteInstallerHasHigherVersion(updateInstallerTask);
        }
    }
}