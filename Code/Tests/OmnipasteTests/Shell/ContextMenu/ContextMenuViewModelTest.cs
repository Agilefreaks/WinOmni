namespace OmnipasteTests.Shell.ContextMenu
{
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using CustomizedClickOnce.Common;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omni;
    using OmniApi.Models;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Shell.ContextMenu;

    [TestFixture]
    public class ContextMenuViewModelTest
    {
        #region Fields

        private Mock<IOmniService> _mockOmniService;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IClickOnceHelper> _mockClickOnceHelper;

        private MoqMockingKernel _mockingKernel;

        private IContextMenuViewModel _subject;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockOmniService = _mockingKernel.GetMock<IOmniService>();
            _mockEventAggregator = _mockingKernel.GetMock<IEventAggregator>();
            _mockClickOnceHelper = _mockingKernel.GetMock<IClickOnceHelper>();

            _mockingKernel.Bind<IContextMenuViewModel>().To<ContextMenuViewModel>();

            _subject = _mockingKernel.Get<IContextMenuViewModel>();
            _subject.ClickOnceHelper = _mockClickOnceHelper.Object;
        }

        [Test]
        public void Constructor_WhenStartupShortcutExists_SetsAutoStartToTrue()
        {
            _subject.AutoStart.Should().BeFalse();
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

        [Test]
        public void ToggleAutoStart_WhenAutoStartIsFalse_CallsRemoveShortcutFromStartup()
        {
            _subject.AutoStart = false;

            _subject.ToggleAutoStart();

            _mockClickOnceHelper.Verify(m => m.RemoveShortcutFromStartup(), Times.Once);
        }

        [Test]
        public void ToggleAutoStart_WhenAutoStartIsTrue_CallsAddShortcutToStartup()
        {
            _subject.AutoStart = true;

            _subject.ToggleAutoStart();

            _mockClickOnceHelper.Verify(m => m.AddShortcutToStartup(), Times.Once);
        }

        #endregion
    }
}