namespace OmnipasteTests.Services
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NAppUpdate.Framework.Sources;
    using NUnit.Framework;
    using OmniCommon;
    using OmniCommon.DataProviders;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Helpers;
    using Omnipaste.Services;
    using Omnipaste.Services.Providers;

    [TestFixture]
    public class UpdaterServiceTests
    {
        private UpdaterService _subject;

        private Mock<IUpdateManager> _mockUpdateManager;

        private Mock<ISystemIdleService> _mockSystemIdleService;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Func<UpdaterService> _createInstance;

        private Mock<IWebProxyFactory> _mockWebProxyFactory;
        
        private Mock<IArgumentsDataProvider> _mockArgumentsDataProvider;

        private Mock<IProcessHelper> _mockProcessHelper;

        private Mock<ILocalInstallerVersionProvider> _mockLocalInstallerVersionProvider;

        private Mock<IRemoteInstallerVersionProvider> _mockRemoteInstallerVersionProvider;

        private Mock<IApplicationVersionProvider> _mockApplicationVersionProvider;

        private TestScheduler _testScheduler;

        private Mock<IApplicationHelper> _mockApplicationHelper;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _mockUpdateManager = new Mock<IUpdateManager> { DefaultValue = DefaultValue.Mock };
            _mockSystemIdleService = new Mock<ISystemIdleService> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            _mockWebProxyFactory = new Mock<IWebProxyFactory> { DefaultValue = DefaultValue.Mock };
            _mockArgumentsDataProvider = new Mock<IArgumentsDataProvider> { DefaultValue = DefaultValue.Mock };
            _mockProcessHelper = new Mock<IProcessHelper> { DefaultValue = DefaultValue.Mock };
            _mockLocalInstallerVersionProvider = new Mock<ILocalInstallerVersionProvider>();
            _mockRemoteInstallerVersionProvider = new Mock<IRemoteInstallerVersionProvider>();
            _mockApplicationVersionProvider = new Mock<IApplicationVersionProvider>();
            _mockApplicationHelper = new Mock<IApplicationHelper>();

            ExternalProcessHelper.Instance = _mockProcessHelper.Object;
            LocalInstallerVersionProvider.Instance = _mockLocalInstallerVersionProvider.Object;
            RemoteInstallerVersionProvider.Instance = _mockRemoteInstallerVersionProvider.Object;
            ApplicationVersionProvider.Instance = _mockApplicationVersionProvider.Object;
            ApplicationHelper.Instance = _mockApplicationHelper.Object;
            _createInstance =
                () =>
                new UpdaterService(
                    _mockUpdateManager.Object,
                    _mockSystemIdleService.Object,
                    _mockConfigurationService.Object,
                    _mockWebProxyFactory.Object,
                    _mockArgumentsDataProvider.Object);
            _subject = _createInstance();
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;

            ExternalProcessHelper.Instance = null;
            LocalInstallerVersionProvider.Instance = null;
            RemoteInstallerVersionProvider.Instance = null;
            ApplicationVersionProvider.Instance = null;
        }

        [Test]
        public void UpdateCheckInterval_ConfigurationHasNoUpdateInterval_IsSetByTheConstructorToTheDefault()
        {
            _mockConfigurationService.SetupGet(x => x[ConfigurationProperties.UpdateInterval]).Returns((string)null);

            _createInstance().UpdateCheckInterval.Should().Be(TimeSpan.FromMinutes(60));
        }

        [Test]
        public void UpdateCheckInterval_ConfigurationHasAnInvalidUpdateInterval_IsSetByTheConstructorToTheDefault()
        {
            _mockConfigurationService.SetupGet(x => x[ConfigurationProperties.UpdateInterval]).Returns("test");

            _createInstance().UpdateCheckInterval.Should().Be(TimeSpan.FromMinutes(60));
        }

        [Test]
        public void UpdateCheckInterval_ConfigurationHasAValidUpdateInterval_IsSetByTheConstructorToTheGivenSettingValue()
        {
            _mockConfigurationService.SetupGet(x => x[ConfigurationProperties.UpdateInterval]).Returns("2");

            _createInstance().UpdateCheckInterval.Should().Be(TimeSpan.FromMinutes(2));
        }

        [Test]
        public void Ctor_Always_SetsTheProxyReturnedByTheProxyFactoryOnTheUpdateSource()
        {
            var mockProxy = new Mock<IWebProxy>();
            IUpdateSource newUpdateSource = null;
            _mockWebProxyFactory.Setup(x => x.CreateFromAppConfiguration()).Returns(mockProxy.Object);
            _mockUpdateManager.SetupSet(m => m.UpdateSource)
                .Callback(updateSource => newUpdateSource = updateSource);

            _createInstance();

            ((SimpleWebSource)newUpdateSource).Proxy.Should().Be(mockProxy.Object);
        }

        [Test]
        public void Start_WhenNewLocalInstallerIsAvailable_NotifiesNewVersionAvailable()
        {
            _mockLocalInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<string>())).Returns(new Version(2, 0, 0));
            _mockApplicationVersionProvider.Setup(m => m.GetVersion()).Returns(new Version(1, 0, 0));
            var testObserver = _testScheduler.CreateObserver<UpdateInfo>();
            _subject.UpdateObservable.Subscribe(testObserver);

            _subject.Start();

            testObserver.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            testObserver.Messages.First().Value.Value.WasInstalled.Should().Be(false);
        }

        [Test]
        public void Start_WhenAppStartedAfterUpdate_NotifiesUpdateListenersThatUpdateWasInstalled()
        {
            _mockArgumentsDataProvider.SetupGet(m => m.Updated).Returns(true);
            UpdateInfo updateInfo = null;
            _subject.UpdateObservable.Subscribe(ui => { updateInfo = ui; }, _ => { });

            _subject.Start();

            updateInfo.WasInstalled.Should().BeTrue();
        }

        [Test]
        public void Start_WhenNewRemoteVersionIsAvailable_InitiatesDownload()
        {
            var updatesAvailableObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(150, Notification.CreateOnCompleted<bool>()));
            _mockUpdateManager.Setup(m => m.AreUpdatesAvailable(It.IsAny<Func<bool>>()))
                .Returns(updatesAvailableObservable);

            _subject.Start();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(20).Ticks);

            _mockUpdateManager.Verify(m => m.DownloadUpdates(It.IsAny<Action>()));
        }

        [Test]
        public void Start_WhenNewRemoteVersionIsAvailable_DownloadsUpdateOnlyOnce()
        {
            var updatesAvailableObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(TimeSpan.FromMinutes(30).Ticks, Notification.CreateOnNext(true)));
            var downloadUpdatesObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(TimeSpan.FromMinutes(30).Ticks, Notification.CreateOnNext(true)));
            _mockUpdateManager.Setup(m => m.AreUpdatesAvailable(It.IsAny<Func<bool>>()))
                .Returns(updatesAvailableObservable);
            _mockUpdateManager.Setup(m => m.DownloadUpdates(It.IsAny<Action>())).Returns<Action>(a => { a(); return downloadUpdatesObservable; });

            _subject.Start();
            _testScheduler.AdvanceBy(TimeSpan.FromMinutes(100).Ticks);

            _mockUpdateManager.Verify(m => m.DownloadUpdates(It.IsAny<Action>()));
        }

        [Test]
        public void Start_WhenNewRemoteVersionIsAvailableAndSystemIsIdle_StartsInstaller()
        {
            var updatesAvailableObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)));
            _mockUpdateManager.Setup(m => m.AreUpdatesAvailable(It.IsAny<Func<bool>>())).Returns(updatesAvailableObservable);
            var downloadUpdatesObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)));
            _mockUpdateManager.Setup(m => m.DownloadUpdates(It.IsAny<Action>())).Returns<Action>(a => { a(); return downloadUpdatesObservable; });
            var systemIdleObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)));
            _mockSystemIdleService.Setup(m => m.CreateSystemIdleObservable(It.IsAny<TimeSpan>())).Returns(systemIdleObservable);
            
            _subject.Start();
            _testScheduler.AdvanceBy(TimeSpan.FromHours(2).Ticks);

            _mockProcessHelper.Verify(m => m.Start(It.IsAny<ProcessStartInfo>()));
        }

        [Test]
        public void Start_WhenAndNewRemoteVersionIsAvailable_NotifiesUpdateAvailableListeners()
        {
            var updatesAvailableObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)));
            _mockUpdateManager.Setup(m => m.AreUpdatesAvailable(It.IsAny<Func<bool>>())).Returns(updatesAvailableObservable);
            var downloadUpdatesObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)));
            _mockUpdateManager.Setup(m => m.DownloadUpdates(It.IsAny<Action>())).Returns<Action>(a => { a(); return downloadUpdatesObservable; });
            var onNextCalled = false;
            _subject.UpdateObservable.Subscribe(_ => { onNextCalled = true; }, _ => { });

            _subject.Start();
            _testScheduler.AdvanceBy(TimeSpan.FromHours(2).Ticks);

            onNextCalled.Should().BeTrue();
        }

        [Test]
        public void NewLocalInstallerAvailable_WhenNoLocalInstallerIsAvailable_ReturnsFalse()
        {
            _mockApplicationVersionProvider.Setup(m => m.GetVersion()).Returns(new Version(1, 0, 0));
            _mockLocalInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<string>())).Returns(null as Version);

            _subject.NewLocalInstallerAvailable().Should().BeFalse();
        }

        [Test]
        public void NewLocalInstallerAvailable_WhenLocalInstallerHasSameVersionAsInstalledApp_ReturnsFalse()
        {
            _mockApplicationVersionProvider.Setup(m => m.GetVersion()).Returns(new Version(1, 0, 0));
            _mockLocalInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<string>())).Returns(new Version(1, 0, 0));

            _subject.NewLocalInstallerAvailable().Should().BeFalse();
        }

        [Test]
        public void NewLocalInstallerAvailable_WhenLocalInstallerHasGreaterVersionThanInstalledApp_ReturnsTrue()
        {
            _mockApplicationVersionProvider.Setup(m => m.GetVersion()).Returns(new Version(1, 0, 0));
            _mockLocalInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<string>())).Returns(new Version(2, 0, 0));
            
            _subject.NewLocalInstallerAvailable().Should().BeTrue();
        }

        [Test]
        public void NewRemoteInstallerAvailable_WhenRemoteVersionIsNull_ReturnsFalse()
        {
            _mockApplicationVersionProvider.Setup(m => m.GetVersion()).Returns(new Version(1, 0, 0));
            _mockRemoteInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<IUpdateManager>(), It.IsAny<string>())).Returns(null as Version);
            _mockLocalInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<string>())).Returns(null as Version);

            _subject.NewRemoteInstallerAvailable().Should().BeFalse();
        }

        [Test]
        public void NewRemoteInstallerAvailable_WhenRemoteVersionIsSameAsInstalled_ReturnsFalse()
        {
            _mockApplicationVersionProvider.Setup(m => m.GetVersion()).Returns(new Version(1, 0, 0));
            _mockRemoteInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<IUpdateManager>(), It.IsAny<string>())).Returns(new Version(1, 0, 0));
            _mockLocalInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<string>())).Returns(null as Version);

            _subject.NewRemoteInstallerAvailable().Should().BeFalse();
        }

        [Test]
        public void NewRemoteInstallerAvailable_WhenRemoteVersionIsGreaterThanInstalledAndLocalIsNull_ReturnsTrue()
        {
            _mockApplicationVersionProvider.Setup(m => m.GetVersion()).Returns(new Version(1, 0, 0));
            _mockRemoteInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<IUpdateManager>(), It.IsAny<string>())).Returns(new Version(2, 0, 0));
            _mockLocalInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<string>())).Returns(null as Version);

            _subject.NewRemoteInstallerAvailable().Should().BeTrue();
        }

        [Test]
        public void NewRemoteInstallerAvailable_WhenRemoteVersionIsGreaterThanInstalledAndSameAsLocal_ReturnsFalse()
        {
            _mockApplicationVersionProvider.Setup(m => m.GetVersion()).Returns(new Version(1, 0, 0));
            _mockRemoteInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<IUpdateManager>(), It.IsAny<string>())).Returns(new Version(2, 0, 0));
            _mockLocalInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<string>())).Returns(new Version(2, 0, 0));

            _subject.NewRemoteInstallerAvailable().Should().BeFalse();
        }

        [Test]
        public void NewRemoteInstallerAvailable_WhenRemoteVersionIsGreaterThanLocalAndRemote_ReturnsTrue()
        {
            _mockApplicationVersionProvider.Setup(m => m.GetVersion()).Returns(new Version(1, 0, 0));
            _mockRemoteInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<IUpdateManager>(), It.IsAny<string>())).Returns(new Version(3, 0, 0));
            _mockLocalInstallerVersionProvider.Setup(m => m.GetVersion(It.IsAny<string>())).Returns(new Version(2, 0, 0));

            _subject.NewRemoteInstallerAvailable().Should().BeTrue();
        }

        [Test]
        public void InstallNewVersion_Always_StopsAllBackgroundProcesses()
        {
            _subject.InstallNewVersion();

            _mockApplicationHelper.Verify(m => m.StopBackgroundProcesses());
        }
    }
}