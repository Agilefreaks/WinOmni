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
    using NAppUpdate.Framework.Sources;
    using OmniCommon;
    using OmniCommon.DataProviders;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Services.Providers;

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

        private readonly IProcessService _processService;

        private readonly TimeSpan _initialUpdateCheckDelay = TimeSpan.FromSeconds(15);

        private readonly TimeSpan _systemIdleThreshold = TimeSpan.FromMinutes(5);

        private readonly IUpdateManager _updateManager;
        
        private readonly ReplaySubject<UpdateInfo> _updateSubject;

        private IDisposable _systemIdleObserver;

        private IDisposable _updateCheckSubscription;

        private IDisposable _proxyConfigurationSubscription;

        #endregion

        #region Constructors and Destructors

        public UpdaterService(
            IUpdateManager updateManager,
            ISystemIdleService systemIdleService,
            IConfigurationService configurationService,
            IWebProxyFactory webProxyFactory,
            IArgumentsDataProvider argumentsDataProvider,
            IProcessService processService)
        {
            SystemIdleService = systemIdleService;
            ConfigurationService = configurationService;
            _updateManager = updateManager;
            _webProxyFactory = webProxyFactory;
            _argumentsDataProvider = argumentsDataProvider;
            _processService = processService;
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
                _processService.Start(new ProcessStartInfo
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
            return ExecuteAndHandleErrorsWithDefault(
                () =>
                    {
                        var newInstallerVersion = GetRemoteInstallerVersion();
                        var localInstallerVersion = GetLocalInstallerVersion();
                        var installedVersion = GetInstalledVersion();

                        return newInstallerVersion != null && newInstallerVersion > installedVersion
                               && (localInstallerVersion == null || newInstallerVersion > localInstallerVersion);
                    },
                false);
        }

        public bool NewLocalInstallerAvailable()
        {
            return ExecuteAndHandleErrorsWithDefault(
                () =>
                    {
                        var installedVersion = GetInstalledVersion();
                        var localInstallerVersion = GetLocalInstallerVersion();

                        return localInstallerVersion != null && localInstallerVersion > installedVersion;
                    },
                false);
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
                _updateCheckSubscription = Disposable.Empty;
            }
            else
            {
                CleanTemporaryFiles();
                _updateCheckSubscription =
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
            DisposeProxyConfigurationSubscription();
            DisposeUpdateCheckSubscription();
        }

        public void OnConfigurationChanged(ProxyConfiguration proxyConfiguration)
        {
            SetUpdateSource();
        }

        #endregion

        #region Methods
        
        private void DisposeSystemIdleObserver()
        {
            if (_systemIdleObserver == null)
            {
                return;
            }

            _systemIdleObserver.Dispose();
            _systemIdleObserver = null;
        }

        private void DisposeProxyConfigurationSubscription()
        {
            if (_proxyConfigurationSubscription == null)
            {
                return;
            }

            _proxyConfigurationSubscription.Dispose();
            _proxyConfigurationSubscription = null;
        }

        private void DisposeUpdateCheckSubscription()
        {
            if (_updateCheckSubscription == null)
            {
                return;
            }
            _updateCheckSubscription.Dispose();
            _updateCheckSubscription = null;
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

        private Version GetLocalInstallerVersion()
        {
            return LocalInstallerVersionProvider.GetVersion(MsiTemporaryPath);
        }

        private Version GetRemoteInstallerVersion()
        {
            return RemoteInstallerVersionProvider.GetVersion(_updateManager, InstallerName);
        }

        private Version GetInstalledVersion()
        {
            return ApplicationVersionProvider.GetVersion();
        }

        private T ExecuteAndHandleErrorsWithDefault<T>(Func<T> executeFunc, T defaultValue)
        {
            T result;
            try
            {
                result = executeFunc();
            }
            catch (Exception exception)
            {
                result = defaultValue;
                ExceptionReporter.Instance.Report(exception);
            }

            return result;
        }

        #endregion
    }
}