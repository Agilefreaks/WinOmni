namespace Omnipaste.Services
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Reflection;
    using Microsoft.Deployment.WindowsInstaller;
    using NAppUpdate.Framework.Sources;
    using OmniCommon;
    using OmniCommon.DataProviders;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class UpdaterService : IUpdaterService
    {
        #region Constants

        private const string InstallerName = "OmnipasteInstaller.msi";

        private const string UpdateFeedFileName = "FeedBuilder.xml";

        private const string ReleaseLogFileName = "Release.md";
        
        private const string MSIExec = "msiexec.exe";

        private const int DefaultUpdateIntervalInMinutes = 60;

        #endregion

        #region Fields

        private readonly IWebProxyFactory _webProxyFactory;

        private readonly IArgumentsDataProvider _argumentsDataProvider;

        private readonly TimeSpan _initialUpdateCheckDelay = TimeSpan.FromSeconds(15);

        private readonly TimeSpan _systemIdleThreshold = TimeSpan.FromMinutes(5);

        private readonly IUpdateManager _updateManager;
        
        private readonly ReplaySubject<UpdateInfo> _updateSubject;

        private IDisposable _systemIdleObserver;

        private IDisposable _updateObserver;

        private IDisposable _proxyConfigurationSubscription;

        #endregion

        #region Constructors and Destructors

        public UpdaterService(
            IUpdateManager updateManager,
            ISystemIdleService systemIdleService,
            IConfigurationService configurationService,
            IWebProxyFactory webProxyFactory,
            IArgumentsDataProvider argumentsDataProvider)
        {
            SystemIdleService = systemIdleService;
            ConfigurationService = configurationService;
            _updateManager = updateManager;
            _webProxyFactory = webProxyFactory;
            _argumentsDataProvider = argumentsDataProvider;
            _updateSubject = new ReplaySubject<UpdateInfo>();

            SetUpdateSource();
            _updateManager.ReinstateIfRestarted();
            UpdateCheckInterval = TimeSpan.FromMinutes(GetUpdateCheckInterval());
        }

        #endregion

        #region Public Properties

        public TimeSpan UpdateCheckInterval { get; private set; }

        public ISystemIdleService SystemIdleService { get; set; }

        public IConfigurationService ConfigurationService { get; set; }

        public IObservable<UpdateInfo> UpdateObservable
        {
            get
            {
                return _updateSubject;
            }
        }

        #endregion

        #region Properties

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
                return Path.Combine(Path.GetTempPath(), Constants.AppName);
            }
        }

        protected string MsiTemporaryPath
        {
            get
            {
                return Path.Combine(InstallerTemporaryFolder, InstallerName);
            }
        }

        protected string ReleaseLogPath
        {
            get
            {
                return Path.Combine(RootDirectory, ReleaseLogFileName);
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

        protected bool IsFirstRunAfterUpdate
        {
            get
            {
                return _argumentsDataProvider.Updated;
            }
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<bool> AreUpdatesAvailable(TimeSpan updateCheckInterval)
        {
            var timer = Observable.Timer(_initialUpdateCheckDelay, updateCheckInterval, SchedulerProvider.Default);
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
                ExceptionReporter.Instance.Report(exception);
            }
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
                ExceptionReporter.Instance.Report(exception);
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

        public bool NewRemoteInstallerAvailable()
        {
            var newInstallerVersion = GetRemoteInstallerVersion(_updateManager);

            return newInstallerVersion != null && RemoteInstallerHasHigherVersion(newInstallerVersion);
        }

        public bool NewLocalInstallerAvailable()
        {
            bool result;
            try
            {
                result = File.Exists(MsiTemporaryPath) && LocalInstallerHasHigherVersion(MsiTemporaryPath);
            }
            catch (Exception exception)
            {
                result = false;
                ExceptionReporter.Instance.Report(exception);
            }

            return result;
        }

        public void Start()
        {
            _proxyConfigurationSubscription =
                ConfigurationService.SettingsChangedObservable.SubscribeToSettingChange<ProxyConfiguration>(
                    ConfigurationProperties.ProxyConfiguration,
                    OnConfigurationChanged);
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
                        .Select(_ => _updateManager.DownloadUpdates(OnDownloadSuccess))
                        .Switch()
                        .ObserveOn(SchedulerProvider.Dispatcher)
                        .SubscribeAndHandleErrors(_ => InstallNewVersionWhenIdle(_systemIdleThreshold));
            }

            if (IsFirstRunAfterUpdate)
            {
                NotifyNewVersion(true);
            }
        }

        private void OnDownloadSuccess()
        {
            MoveUpdatesToTempFolder();
            NotifyNewVersion();
        }

        public void Stop()
        {
            if (_proxyConfigurationSubscription != null)
            {
                _proxyConfigurationSubscription.Dispose();
                _proxyConfigurationSubscription = null;
            }

            _updateObserver.Dispose();
        }

        public void OnConfigurationChanged(ProxyConfiguration proxyConfiguration)
        {
            SetUpdateSource();
        }

        #endregion

        #region Methods

        private static string GetLocalInstallerVersion(string msiPath)
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
        private static string GetRemoteInstallerVersion(IUpdateManager updateManager)
        {
            return updateManager.GetUpdatedFiles()
                    .Where(fileUpdateTask => fileUpdateTask.LocalPath == InstallerName)
                    .Select(fileUpdateTask => fileUpdateTask.Version)
                    .FirstOrDefault();
        }

        private static bool LocalInstallerHasHigherVersion(string msiPath)
        {
            return VersionIsHigherThanOwn(GetLocalInstallerVersion(msiPath));
        }

        private static bool RemoteInstallerHasHigherVersion(string version)
        {
            return VersionIsHigherThanOwn(version);
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

        private void MoveUpdatesToTempFolder()
        {
            try
            {
                if (!Directory.Exists(InstallerTemporaryFolder))
                {
                    Directory.CreateDirectory(InstallerTemporaryFolder);
                }

                var localFiles = _updateManager.GetUpdatedFiles().Select(m => m.LocalPath).ToList();

                _updateManager.ApplyUpdates(false);

                //Copy updates to a temp file as the app directory might get uninstalled
                localFiles.ForEach(
                        localPath =>
                            {
                                var filePath = Path.Combine(RootDirectory, localPath);
                                var newFilePath = Path.Combine(InstallerTemporaryFolder, localPath);
                                
                                File.Copy(filePath, newFilePath, true);
                            });
            }
            catch (Exception exception)
            {
                ExceptionReporter.Instance.Report(exception);
                throw;
            }
        }

        private void NotifyNewVersion(bool wasInstalled = false)
        {
            var updateInfo = new UpdateInfo
                                        {
                                            WasInstalled = wasInstalled,
                                            ReleaseLog = File.Exists(ReleaseLogPath) ? File.ReadAllText(ReleaseLogPath) : string.Empty
                                        };
            _updateSubject.OnNext(updateInfo);
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