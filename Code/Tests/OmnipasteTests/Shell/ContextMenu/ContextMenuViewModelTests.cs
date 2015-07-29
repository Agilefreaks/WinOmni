namespace OmnipasteTests.Shell.ContextMenu
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omni;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework.EventAggregatorMessages;
    using Omnipaste.Shell.ContextMenu;
    using SQLitePCL;

    [TestFixture]
    public class ContextMenuViewModelTests
    {
        #region Fields

        private Mock<IOmniService> _mockOmniService;

        private Mock<IEventAggregator> _mockEventAggregator;

        private MoqMockingKernel _mockingKernel;

        private IContextMenuViewModel _subject;

        private Mock<IApplicationService> _mockApplicationService;

        private Mock<IConfigurationService> _mockConfigurationService;

        private TestScheduler _testScheduler;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _mockOmniService = _mockingKernel.GetMock<IOmniService>();
            _mockOmniService.SetupGet(os => os.StatusChangedObservable).Returns(Observable.Empty<OmniServiceStatusEnum>());
            _mockEventAggregator = _mockingKernel.GetMock<IEventAggregator>();
            _mockApplicationService = _mockingKernel.GetMock<IApplicationService>();
            _mockConfigurationService = _mockingKernel.GetMock<IConfigurationService>();

            _mockingKernel.Bind<IConfigurationService>().ToConstant(_mockConfigurationService.Object);
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
            var testableObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<OmniServiceStatusEnum>>(
                    100,
                    Notification.CreateOnNext(OmniServiceStatusEnum.Started)));
            _mockOmniService.SetupGet(os => os.StatusChangedObservable).Returns(testableObservable);

            var viewModel = _mockingKernel.Get<IContextMenuViewModel>();
            viewModel.IconSource = "test";
            _testScheduler.Start();

            viewModel.IconSource.Should().Be("/Connected.ico");
        }

        [Test]
        public void OmniService_StatusChangedToStopped_SetsTheIconSourceToDisconnected()
        {
            var testableObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<OmniServiceStatusEnum>>(
                    100,
                    Notification.CreateOnNext(OmniServiceStatusEnum.Stopped)));
            _mockOmniService.SetupGet(os => os.StatusChangedObservable).Returns(testableObservable);
            
            var viewModel = _mockingKernel.Get<IContextMenuViewModel>();
            viewModel.IconSource = "test";
            _testScheduler.Start();

            viewModel.IconSource.Should().Be("/Disconnected.ico");
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
        public void SetPause_Always_SetsConfigurationServiceProperty()
        {
             _subject.Pause = true;

            _mockConfigurationService.VerifySet(mock => mock.PauseNotifications = true, Times.Once);
        }

        #endregion
    }
}