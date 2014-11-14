namespace OmnipasteTests.Shell.ContextMenu
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omni;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Services.Monitors.User;
    using Omnipaste.Shell.ContextMenu;

    [TestFixture]
    public class ContextMenuViewModelTests
    {
        #region Fields

        private Mock<IOmniService> _mockOmniService;

        private Mock<IEventAggregator> _mockEventAggregator;

        private MoqMockingKernel _mockingKernel;

        private IContextMenuViewModel _subject;

        private Subject<OmniServiceStatusEnum> _statusChangedSubject;

        private Mock<IApplicationService> _mockApplicationService;

        private Mock<IUserMonitor> _mockUserMonitor;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockOmniService = _mockingKernel.GetMock<IOmniService>();
            _mockOmniService.Setup(x => x.InTransitionObservable).Returns(Observable.Empty<bool>());
            _statusChangedSubject = new Subject<OmniServiceStatusEnum>();
            _mockOmniService.SetupGet(os => os.StatusChangedObservable).Returns(_statusChangedSubject);
            _mockEventAggregator = _mockingKernel.GetMock<IEventAggregator>();
            _mockApplicationService = _mockingKernel.GetMock<IApplicationService>();
            _mockUserMonitor = _mockingKernel.GetMock<IUserMonitor>();

            _mockingKernel.Bind<IContextMenuViewModel>().To<ContextMenuViewModel>();
            _mockingKernel.Bind<IUserMonitor>().ToConstant(_mockUserMonitor.Object);           

            _subject = _mockingKernel.Get<IContextMenuViewModel>();
        }

        [Test]
        public void Constructor_WhenStartupShortcutExists_SetsAutoStartToTrue()
        {
            _subject.AutoStart.Should().BeFalse();
        }

        [Test]
        public void Constructor_SetsIconToDisconnected()
        {
            _subject.IconSource.Should().Be("/Disconnected.ico");
        }

        [Test]
        public void OmniService_StatusChangedToStarted_SetsTheIconSourceToConnected()
        {
            _statusChangedSubject.OnNext(OmniServiceStatusEnum.Started);

            _subject.IconSource.Should().Be("/Connected.ico");
        }

        [Test]
        public void OmniService_StatusChangedToStopped_SetsTheIconSourceToDisconnected()
        {
            _statusChangedSubject.OnNext(OmniServiceStatusEnum.Stopped);

            _subject.IconSource.Should().Be("/Disconnected.ico");
        }

        [Test]
        public void ToggleSync_WhenIsStoppedIsTrue_PublishesDisconnectEvent()
        {
            _subject.IsStopped = true;

            _subject.ToggleSync();

            _mockUserMonitor.Verify(x => x.SendEvent(UserEventTypeEnum.Disconnect));
        }

        [Test]
        public void ToggleSync_WhenIsStoppedIsFalse_PublishesConnectEvent()
        {
            _subject.IsStopped = false;

            _subject.ToggleSync();

            _mockUserMonitor.Verify(x => x.SendEvent(UserEventTypeEnum.Connect));
        }

        [Test]
        public void Show_Always_PublishesShowShellEvent()
        {
            _subject.Show();

            _mockEventAggregator.Verify(m => m.Publish(It.IsAny<ShowShellMessage>(), Execute.OnUIThread));
        }

        [Test]
        public void SetAutoStart_WhenAutoStartIsFalseAndNewValueIsTrue_SetsApplicationServiceAutoStartToTrue()
        {
            _mockApplicationService.SetupGet(x => x.AutoStart).Returns(false);

            _subject.AutoStart = true;

            _mockApplicationService.VerifySet(x => x.AutoStart = true, Times.Once);
        }

        [Test]
        public void SetAutoStart_WhenAutoStartIsTrueAndNewValueIsFalse_SetsApplicationServiceAutoStartToFalse()
        {
            _mockApplicationService.SetupGet(x => x.AutoStart).Returns(true);

            _subject.AutoStart = false;

            _mockApplicationService.VerifySet(x => x.AutoStart = false, Times.Once);
        }

        [Test]
        public void TooltipText_HasVersion()
        {
            _mockApplicationService.SetupGet(s => s.Version).Returns(new Version("1.0.1.10"));

            _subject.TooltipText.Should().Be("Omnipaste 1.0.1.10");
        }

        #endregion
    }
}