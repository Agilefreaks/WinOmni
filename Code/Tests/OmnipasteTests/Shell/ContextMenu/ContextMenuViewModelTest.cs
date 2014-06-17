namespace OmnipasteTests.Shell.ContextMenu
{
    using System;
    using Caliburn.Micro;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Interfaces;
    using Omnipaste.Shell.ContextMenu;
    using Action = System.Action;

    [TestFixture]
    public class ContextMenuViewModelTest
    {
        #region Fields

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IConfigurationService> _mockConfigurationService;

        private MoqMockingKernel _mockingKernel;

        private IContextMenuViewModel _subject;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockingKernel.Bind<IEventAggregator>().ToConstant(_mockEventAggregator.Object);

            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockingKernel.Bind<IConfigurationService>().ToConstant(_mockConfigurationService.Object);

            _mockingKernel.Bind<IContextMenuViewModel>().To<ContextMenuViewModel>();

            _subject = _mockingKernel.Get<IContextMenuViewModel>();
        }

        [Test]
        public void ToggleSync_WhenIsSyncingIsTrue_PublishesStartOmniServiceMessage()
        {
            _subject.IsSyncing = true;

            _subject.ToggleSync();

            _mockEventAggregator.Verify(
                m => m.Publish(It.IsAny<StartOmniServiceMessage>(), It.IsAny<Action<Action>>()),
                Times.Once);
        }

        [Test]
        public void ToggleSync_WhenIsSyncingIsFalse_PublishesStopOmniServiceMessage()
        {
            _subject.IsSyncing = false;

            _subject.ToggleSync();

            _mockEventAggregator.Verify(
                m => m.Publish(It.IsAny<StopOmniServiceMessage>(), It.IsAny<Action<Action>>()),
                Times.Once);
        }

        #endregion
    }
}