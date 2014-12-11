namespace OmnipasteTests.Shell.SessionInfo
{
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Omni;
    using OmniCommon.Interfaces;
    using OmniCommon;
    using OmniCommon.Helpers;
    using OmniCommon.Models;
    using OmniCommon.Settings;
    using System.Reactive;
    using Omnipaste.Shell.SessionInfo;

    [TestFixture]
    public class SessionInfoViewModelTests
    {
        private SessionInfoViewModel _subject;
        private TestScheduler _scheduler;
        private Mock<IOmniService> _mockOmniService;
        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void SetUp()
        {
            _scheduler = new TestScheduler();
            SchedulerProvider.Default = _scheduler;
            SchedulerProvider.Dispatcher = _scheduler;

            _mockOmniService = new Mock<IOmniService>();
            _mockOmniService.SetupGet(x => x.StatusChangedObservable).Returns(_scheduler.CreateColdObservable<OmniServiceStatusEnum>());
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockConfigurationService.SetupGet(x => x.SettingsChangedObservable).Returns(_scheduler.CreateColdObservable<SettingsChangedData>());

            _subject = new SessionInfoViewModel(_mockOmniService.Object, _mockConfigurationService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void ConfigurationService_WhenSettingsChangedObservablesTriggersOnNext_UpdatesUserInfo()
        {
            var newUserInfo = new UserInfo();
            var settingsChangedObservable = _scheduler.CreateColdObservable(
                new Recorded<Notification<SettingsChangedData>>(100,
                    Notification.CreateOnNext(new SettingsChangedData { SettingName = ConfigurationProperties.UserInfo, NewValue = newUserInfo })),
                new Recorded<Notification<SettingsChangedData>>(200,
                        Notification.CreateOnCompleted<SettingsChangedData>()));
            _mockConfigurationService.SetupGet(x => x.SettingsChangedObservable).Returns(settingsChangedObservable);
            _subject = new SessionInfoViewModel(_mockOmniService.Object, _mockConfigurationService.Object);
            
            _scheduler.Start();

            _subject.UserInfo.Should().Be(newUserInfo);
        }
    }
}
