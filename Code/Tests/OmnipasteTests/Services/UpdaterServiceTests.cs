namespace OmnipasteTests.Services
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NAppUpdate.Framework.Sources;
    using NUnit.Framework;
    using OmniCommon;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;

    [TestFixture]
    public class UpdaterServiceTests
    {
        private UpdaterService _subject;

        private Mock<IUpdateManager> _mockUpdateManager;

        private Mock<ISystemIdleService> _mockSystemIdleService;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Func<UpdaterService> _createInstance;

        private Mock<IWebProxyFactory> _mockWebProxyFactory;

        private TestScheduler _testScheduler;

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
            _createInstance =
                () =>
                new UpdaterService(
                    _mockUpdateManager.Object,
                    _mockSystemIdleService.Object,
                    _mockConfigurationService.Object,
                    _mockWebProxyFactory.Object);
            _subject = _createInstance();
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
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
        public void Start_WhenThereAreNoLocalUpdatesAndNewRemoteVersionIsAvailable_InitiatesDownload()
        {
            var updatesAvailableObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(150, Notification.CreateOnCompleted<bool>()));
            _mockUpdateManager.Setup(m => m.AreUpdatesAvailable(It.IsAny<Func<bool>>()))
                .Returns(updatesAvailableObservable);

            _subject.Start();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(20).Ticks);

            _mockUpdateManager.Verify(m => m.DownloadUpdates());
        }

        [Test]
        public void Start_WhenThereAreNoLocalUpdatesAndNewRemoteVersionIsAvailable_NotifiesUpdateAvailableListeners()
        {
            var updatesAvailableObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(150, Notification.CreateOnCompleted<bool>()));
            var downloadUpdatesObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(150, Notification.CreateOnCompleted<bool>()));
            _mockUpdateManager.Setup(m => m.AreUpdatesAvailable(It.IsAny<Func<bool>>())).Returns(updatesAvailableObservable);
            _mockUpdateManager.Setup(m => m.DownloadUpdates()).Returns(downloadUpdatesObservable);
            var onNextCalled = false;
            _subject.UpdateAvailableObservable.Subscribe(_ => { onNextCalled = true; }, _ => { });

            _subject.Start();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(20).Ticks);

            onNextCalled.Should().BeTrue();
        }
    }
}