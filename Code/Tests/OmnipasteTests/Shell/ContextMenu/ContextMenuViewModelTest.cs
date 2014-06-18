namespace OmnipasteTests.Shell.ContextMenu
{
    using Caliburn.Micro;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omni;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Shell.ContextMenu;

    [TestFixture]
    public class ContextMenuViewModelTest
    {
        #region Fields

        private Mock<IOmniService> _mockOmniService;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IEventAggregator> _mockEventAggregator;

        private MoqMockingKernel _mockingKernel;

        private IContextMenuViewModel _subject;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockOmniService = _mockingKernel.GetMock<IOmniService>();
            _mockConfigurationService = _mockingKernel.GetMock<IConfigurationService>();
            _mockEventAggregator = _mockingKernel.GetMock<IEventAggregator>();

            _mockingKernel.Bind<IContextMenuViewModel>().To<ContextMenuViewModel>();

            _subject = _mockingKernel.Get<IContextMenuViewModel>();
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
            _subject.IsStopped = false;

            _subject.ToggleSync();

            _mockOmniService.Verify(
                m => m.Start(null),
                Times.Once);
        }

        [Test]
        public void Show_Always_PublishesShowShellEvent()
        {
            _subject.Show();

            _mockEventAggregator.Verify(m => m.Publish(It.IsAny<ShowShellMessage>(), Execute.OnUIThread));
        }

        [Test]
        public void ToggleAutoStart_Always_SetConfiguration()
        {
            _mockConfigurationService.SetupGet(m => m.AutoStart).Returns(true);

            _subject.ToggleAutoStart();

            _mockConfigurationService.VerifySet(m => m.AutoStart = false);
        }

        #endregion
    }
}