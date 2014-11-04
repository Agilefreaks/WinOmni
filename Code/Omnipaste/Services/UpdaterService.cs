namespace Omnipaste.Services
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reflection;
    using BugFreak;
    using Microsoft.Deployment.WindowsInstaller;
    using NAppUpdate.Framework;
    using NAppUpdate.Framework.Sources;
    using NAppUpdate.Framework.Tasks;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Framework;

    public class UpdaterService : IUpdaterService
    {
        private readonly IWebProxyFactory _webProxyFactory;

        #region Constants

        private const string InstallerName = "OmnipasteInstaller.msi";

        private const string MSIExec = "msiexec.exe";

        private const string UpdateFeedFileName = "FeedBuilder.xml";

        private const int DefaultUpdateIntervalInMinutes = 60;

        #endregion

        #region Fields

        private readonly TimeSpan _initialUpdateCheckDelay = TimeSpan.FromSeconds(15);

        private readonly TimeSpan _systemIdleThreshold = TimeSpan.FromMinutes(5);

        private readonly UpdateManager _updateManager;

        private IDisposable _systemIdleObserver;

        private IDisposable _updateObserver;

        #endregion

        #region Constructors and Destructors

        public UpdaterService(
            ISystemIdleService systemIdleService,
            IConfigurationService configurationService,
            IWebProxyFactory webProxyFactory)
        {
            SystemIdleService = systemIdleService;
            ConfigurationService = configurationService;
            _updateManager = UpdateManager.Instance;
            _webProxyFactory = webProxyFactory;

            SetUpdateSource();
            _updateManager.ReinstateIfRestarted();
            UpdateCheckInterval = TimeSpan.FromMinutes(GetUpdateCheckInterval());
            ConfigurationService.AddProxyConfigurationObserver(this);
        }

        #endregion

        #region Public Properties

        public TimeSpan UpdateCheckInterval { get; private set; }

        public ISystemIdleService SystemIdleService { get; set; }

        public IConfigurationService ConfigurationService { get; set; }

        #endregion

        #region Properties

        protected string AppName
        {
            get
            {
                return ConfigurationService[ConfigurationProperties.AppName];
            }
        }

        protected string FeedUrl
        {
            get
            {
                return string.Format(
                    "{0}{1}",
                    ConfigurationService[ConfigurationProperties.UpdateSource],
                    UpdateFeedFileName);
            }
        }

        protected string InstallerTemporaryFolder
        {
            get
            {
                return Path.Combine(Path.GetTempPath(), AppName);
            }
        }

        protected string MsiTemporaryPath
        {
            get
            {
                return Path.Combine(InstallerTemporaryFolder, InstallerName);
            }
        }

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

        #endregion

        #region Public Methods and Operators

        public IObservable<bool> AreUpdatesAvailable(TimeSpan updateCheckInterval)
        {
            var timer = Observable.Timer(_initialUpdateCheckDelay, updateCheckInterval);
            return timer.Select(_ => _updateManager.AreUpdatesAvailable(NewRemoteInstallerAvailable)).Switch();
        }

        public void CleanTemporaryFiles()
        {
            try
            {
                if (Directory.Exists(InstallerTemporaryFolder))
                {
                    Directory.Delete(InstallerTemporaryFolder, true);
                }
            }
            catch (Exception exception)
            {
                ReportingService.Instance.BeginReport(exception);
            }
        }

        public IObservable<bool> DownloadUpdates()
        {
            return _updateManager.DownloadUpdates(PrepareDownloadedInstaller);
        }

        public void InstallNewVersion()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                                  {
                                      FileName = MSIExec,
                                      Arguments = string.Format("/i {0} /qn /l*v LogFile.txt", MsiTemporaryPath),
                                      WorkingDirectory = InstallerTemporaryFolder
                                  });
            }
            catch (Exception exception)
            {
                ReportingService.Instance.BeginReport(exception);
            }
        }

        public void InstallNewVersionWhenIdle(TimeSpan systemIdleThreshold)
        {
            DisposeSystemIdleObserver();
            _systemIdleObserver =
                SystemIdleService.CreateSystemIdleObservable(systemIdleThreshold)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .Where(systemIsIdle => systemIsIdle)
                    .SubscribeAndHandleErrors(
                        _ =>
                        {
                            DisposeSystemIdleObserver();
                            InstallNewVersion();
                        });
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

        public void Start()
        {
            if (NewLocalInstallerAvailable())
            {
                InstallNewVersion();
                _updateObserver = Disposable.Empty;
            }
            else
            {
                CleanTemporaryFiles();
                _updateObserver =
                    AreUpdatesAvailable(UpdateCheckInterval)
                        .Where(updateAvailable => updateAvailable)
                        .Select(_ => DownloadUpdates())
                        .Switch()
                        .ObserveOn(SchedulerProvider.Dispatcher)
                        .SubscribeAndHandleErrors(
                            _ => InstallNewVersionWhenIdle(_systemIdleThreshold),
                            ReportingService.Instance.BeginReport);
            }
        }

        public void Stop()
        {
            _updateObserver.Dispose();
        }

        public void OnConfigurationChanged(ProxyConfiguration proxyConfiguration)
        {
            SetUpdateSource();
        }

        #endregion

        #region Methods

        private static string GetMsiVersion(string msiPath)
        {
            string versionString;
            using (var database = new Database(msiPath))
            {
                versionString =
                    database.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = '{0}'", "ProductVersion")
                    as string;
            }

            return versionString;
        }

        private static bool MsiHasHigherVersion(string msiPath)
        {
            return VersionIsHigherThanOwn(GetMsiVersion(msiPath));
        }

        private static bool RemoteInstallerHasHigherVersion(FileUpdateTask installerUpdateTask)
        {
            return VersionIsHigherThanOwn(installerUpdateTask.Version);
        }

        private static bool VersionIsHigherThanOwn(string versionString)
        {
            Version msiVersion;
            Version.TryParse(versionString, out msiVersion);
            var exeVersion = Assembly.GetEntryAssembly().GetName().Version;

            return msiVersion > exeVersion;
        }

        private void DisposeSystemIdleObserver()
        {
            if (_systemIdleObserver != null)
            {
                _systemIdleObserver.Dispose();
            }
        }

        private FileUpdateTask GetUpdateInstallerTask()
        {
            return
                _updateManager.Tasks.Where(task => task is FileUpdateTask)
                    .Cast<FileUpdateTask>()
                    .FirstOrDefault(fileUpdateTask => fileUpdateTask.LocalPath == InstallerName);
        }

        private bool NewRemoteInstallerAvailable()
        {
            var updateInstallerTask = GetUpdateInstallerTask();

            return updateInstallerTask != null && RemoteInstallerHasHigherVersion(updateInstallerTask);
        }

        private void PrepareDownloadedInstaller()
        {
            try
            {
                var updateInstallerTask = GetUpdateInstallerTask();
                _updateManager.ApplyUpdates(false);
                Directory.CreateDirectory(InstallerTemporaryFolder);
                //Move new msi to a temp file as the app directory might get uninstalled
                var installerPath = Path.Combine(RootDirectory, updateInstallerTask.LocalPath);
                File.Copy(installerPath, MsiTemporaryPath, true);
            }
            catch (Exception exception)
            {
                ReportingService.Instance.BeginReport(exception);
                throw;
            }
        }

        private int GetUpdateCheckInterval()
        {
            int timeoutInMinutes;
            var intervalSettingValue = ConfigurationService[ConfigurationProperties.UpdateInterval];
            if (!int.TryParse(intervalSettingValue, out timeoutInMinutes))
            {
                timeoutInMinutes = DefaultUpdateIntervalInMinutes;
            }

            return timeoutInMinutes;
        }

        private void SetUpdateSource()
        {
            var proxy = _webProxyFactory.CreateFromAppConfiguration();
            _updateManager.UpdateSource = new SimpleWebSource(FeedUrl) { Proxy = proxy };
        }

        #endregion
    }
}