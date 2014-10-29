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
    using OmniApi.Models;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Shell.ContextMenu;
    using OmniSync;

    [TestFixture]
    public class ContextMenuViewModelTests
    {
        #region Fields

        private Mock<IOmniService> _mockOmniService;

        private Mock<IEventAggregator> _mockEventAggregator;

        private MoqMockingKernel _mockingKernel;

        private IContextMenuViewModel _subject;

        private Subject<ServiceStatusEnum> _statusChangedSubject;

        private Mock<IApplicationService> _mockApplicationService;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockOmniService = _mockingKernel.GetMock<IOmniService>();
            _statusChangedSubject = new Subject<ServiceStatusEnum>();
            _mockOmniService.SetupGet(os => os.StatusChangedObservable).Returns(_statusChangedSubject);
            _mockEventAggregator = _mockingKernel.GetMock<IEventAggregator>();
            _mockApplicationService = _mockingKernel.GetMock<IApplicationService>();

            _mockingKernel.Bind<IContextMenuViewModel>().To<ContextMenuViewModel>();

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
            _statusChangedSubject.OnNext(ServiceStatusEnum.Started);

            _subject.IconSource.Should().Be("/Connected.ico");
        }

        [Test]
        public void OmniService_StatusChangedToStopped_SetsTheIconSourceToDisconnected()
        {
            _statusChangedSubject.OnNext(ServiceStatusEnum.Stopped);

            _subject.IconSource.Should().Be("/Disconnected.ico");
        }

        [Test]
        public void ToggleSync_WhenIsSyncingIsTrue_PublishesStartOmniServiceMessage()
        {
            _subject.IsStopped = true;

            _subject.ToggleSync();

            _mockOmniService.Verify(
                m => m.Stop(true),
                Times.Once);
        }

        [Test]
        public void ToggleSync_WhenIsSyncingIsFalse_PublishesStopOmniServiceMessage()
        {
            _mockOmniService.Setup(m => m.Start()).Returns(Observable.Empty<Device>());
            _subject.IsStopped = false;

            _subject.ToggleSync();

            _mockOmniService.Verify(m => m.Start(), Times.Once);
        }

        [Test]
        public void Show_Always_PublishesShowShellEvent()
        {
            _subject.Show();

            _mockEventAggregator.Verify(m => m.Publish(It.IsAny<ShowShellMessage>(), Execute.OnUIThread));
        }

        //[Test]
        //public void ToggleAutoStart_WhenAutoStartIsFalse_CallsRemoveShortcutFromStartup()
        //{
        //    _subject.AutoStart = false;

        //    _subject.ToggleAutoStart();

        //    _mockClickOnceHelper.Verify(m => m.RemoveShortcutFromStartup(), Times.Once);
        //}

        //[Test]
        //public void ToggleAutoStart_WhenAutoStartIsTrue_CallsAddShortcutToStartup()
        //{
        //    _subject.AutoStart = true;

        //    _subject.ToggleAutoStart();

        //    _mockClickOnceHelper.Verify(m => m.AddShortcutToStartup(), Times.Once);
        //}

        [Test]
        public void TooltipText_HasVersion()
        {
            _mockApplicationService.SetupGet(s => s.Version).Returns(new Version("1.0.1.10"));

            _subject.TooltipText.Should().Be("Omnipaste 1.0.1.10");
        }

        #endregion
    }
}