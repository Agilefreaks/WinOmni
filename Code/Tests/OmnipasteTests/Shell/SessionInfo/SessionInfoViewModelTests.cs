namespace OmnipasteTests.Shell.SessionInfo
{
    using System.Windows.Media;
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
    using Omnipaste.Presenters;
    using Omnipaste.Shell.SessionInfo;
    using OmniUI.Helpers;

    [TestFixture]
    public class SessionInfoViewModelTests
    {
        private SessionInfoViewModel _subject;
        private TestScheduler _scheduler;
        private Mock<IOmniService> _mockOmniService;
        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IApplicationHelper> _mockApplicationHelper;

        [SetUp]
        public void SetUp()
        {
            _scheduler = new TestScheduler();
            SchedulerProvider.Default = _scheduler;
            SchedulerProvider.Dispatcher = _scheduler;

            _mockOmniService = new Mock<IOmniService>();
            _mockOmniService.SetupGet(x => x.StatusChangedObservable).Returns(_scheduler.CreateColdObservable<OmniServiceStatusEnum>());
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService.SetupGet(x => x.SettingsChangedObservable).Returns(_scheduler.CreateColdObservable<SettingsChangedData>());
            _mockApplicationHelper = new Mock<IApplicationHelper>();
            _mockApplicationHelper.Setup(x => x.FindResource(ContactInfoPresenter.UserPlaceholderBrush))
                .Returns(new DrawingBrush(new DrawingGroup()));
            ApplicationHelper.Instance = _mockApplicationHelper.Object;

            _subject = new SessionInfoViewModel(_mockOmniService.Object, _mockConfigurationService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
            ApplicationHelper.Instance = null;
        }

        [Test]
        public void ConfigurationService_WhenSettingsChangedObservablesTriggersOnNext_UpdatesUserInfo()
        {
            var newUserInfo = new UserInfo { FirstName = "Test", LastName = "Last" };
            var settingsChangedObservable = _scheduler.CreateColdObservable(
                new Recorded<Notification<SettingsChangedData>>(100,
                    Notification.CreateOnNext(new SettingsChangedData { SettingName = ConfigurationProperties.UserInfo, NewValue = newUserInfo })),
                new Recorded<Notification<SettingsChangedData>>(200,
                        Notification.CreateOnCompleted<SettingsChangedData>()));
            _mockConfigurationService.SetupGet(x => x.SettingsChangedObservable).Returns(settingsChangedObservable);
            _subject = new SessionInfoViewModel(_mockOmniService.Object, _mockConfigurationService.Object);
            var oldUserInfo = _subject.UserInfo;

            _scheduler.Start();

            _subject.UserInfo.Should().NotBe(oldUserInfo);
            _subject.UserInfo.Identifier.Should().Be(newUserInfo.FullName());
        }
    }
}
