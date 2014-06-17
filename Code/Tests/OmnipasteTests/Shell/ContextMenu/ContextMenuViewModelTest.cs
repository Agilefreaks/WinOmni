namespace OmnipasteTests.Shell.ContextMenu
{
    using System;
    using Caliburn.Micro;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omni;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Interfaces;
    using Omnipaste.Shell.ContextMenu;
    using Action = System.Action;

    [TestFixture]
    public class ContextMenuViewModelTest
    {
        #region Fields

        private Mock<IOmniService> _mockOmniService;

        private Mock<IConfigurationService> _mockConfigurationService;

        private MoqMockingKernel _mockingKernel;

        private IContextMenuViewModel _subject;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockOmniService = new Mock<IOmniService>();
            _mockingKernel.Bind<IOmniService>().ToConstant(_mockOmniService.Object);

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

            _mockOmniService.Verify(
                m => m.Start(null),
                Times.Once);
        }

        [Test]
        public void ToggleSync_WhenIsSyncingIsFalse_PublishesStopOmniServiceMessage()
        {
            _subject.IsSyncing = false;

            _subject.ToggleSync();

            _mockOmniService.Verify(
                m => m.Stop(true),
                Times.Once);
        }

        #endregion
    }
}