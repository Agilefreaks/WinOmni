namespace OmnipasteTests.Shell.SessionInfo
{
    using System;
    using System.Reactive;
    using System.Windows.Media;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Omni;
    using OmniCommon;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniCommon.Settings;
    using Omnipaste.Framework.Models;
    using Omnipaste.Profile;
    using Omnipaste.Shell.SessionInfo;
    using OmniUI.Framework.Helpers;
    using OmniUI.Workspaces;

    [TestFixture]
    public class SessionInfoViewModelTests
    {
        private SessionInfoViewModel _subject;
        private TestScheduler _scheduler;
        private Mock<IOmniService> _mockOmniService;
        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IResourceHelper> _mockResourceHelper;

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
            _mockResourceHelper = new Mock<IResourceHelper>();
            _mockResourceHelper.Setup(x => x.GetByKey(ContactModel.UserPlaceholderBrush))
                .Returns(new DrawingBrush(new DrawingGroup()));
            ResourceHelper.Instance = _mockResourceHelper.Object;

            _subject = new SessionInfoViewModel(_mockOmniService.Object, _mockConfigurationService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
            ResourceHelper.Instance = null;
        }

        [Test]
        public void WhenSettingsChangedObservablesTriggersOnNext_Always_UpdatesUserInfo()
        {
            var newUserInfo = UserInfo.BeginBuild().WithFirstName("Test").WithLastName("Last");
            var settingsChangedObservable = _scheduler.CreateColdObservable(
                new Recorded<Notification<SettingsChangedData>>(100,
                    Notification.CreateOnNext(new SettingsChangedData { SettingName = ConfigurationProperties.UserInfo, NewValue = newUserInfo })),
                new Recorded<Notification<SettingsChangedData>>(200,
                        Notification.CreateOnCompleted<SettingsChangedData>()));
            _mockConfigurationService.SetupGet(x => x.SettingsChangedObservable).Returns(settingsChangedObservable);
            _subject = new SessionInfoViewModel(_mockOmniService.Object, _mockConfigurationService.Object);
            var oldUserInfo = _subject.User;

            _scheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.User.Should().NotBe(oldUserInfo);
            _subject.User.Identifier.Should().Be("Test Last");
        }

        [Test]
        public void ShowUserProfile_Always_ActivatesProfileWorkspaceInWorkspaceConductor()
        {
            var profileWorkspace = new Mock<IProfileWorkspace>();
            var workspaceConductor = new Mock<IWorkspaceConductor>();

            _subject.ProfileWorkspace = profileWorkspace.Object;
            _subject.WorkspaceConductor = workspaceConductor.Object;

            _subject.ShowUserProfile();

            workspaceConductor.Verify(mock => mock.ActivateItem(profileWorkspace.Object), Times.Once);
        }
    }
}
