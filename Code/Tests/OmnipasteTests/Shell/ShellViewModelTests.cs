namespace OmnipasteTests.Shell
{
    using Caliburn.Micro;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.Dialog;
    using Omnipaste.Loading.UserToken;
    using Omnipaste.Shell;
    using Omnipaste.Shell.ContextMenu;

    [TestFixture]
    public class ShellViewModelTests
    {
        #region Fields

        private Mock<IContextMenuViewModel> _mockContextViewModel;

        private Mock<IDialogViewModel> _mockDialogViewModel;

        private Mock<IUserTokenViewModel> _mockUserTokenViewModel;

        private ShellViewModel _subject;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<ISessionManager> _mockSessionManager;

        #endregion

        [SetUp]
        public void SetUp()
        {
            var kernel = new MoqMockingKernel();

            _mockDialogViewModel = kernel.GetMock<IDialogViewModel>();
            _mockUserTokenViewModel = kernel.GetMock<IUserTokenViewModel>();
            _mockContextViewModel = kernel.GetMock<IContextMenuViewModel>();
            kernel.Bind<IEventAggregator>().ToMock();
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            kernel.Bind<ISessionManager>().ToConstant(_mockSessionManager.Object);

            kernel.Bind<IShellViewModel>().To<ShellViewModel>();

            _subject = kernel.Get<ShellViewModel>();
        }

        [Test]
        public void Close_CallsContextMenuViewModelToShowNotificationBubble()
        {
            _subject.Close();

            _mockContextViewModel.Verify(cvm => cvm.ShowBaloon(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Close_WithoutShowBaloonSetToFalse_WillNotCallContextMenuViewModelShowBaloon()
        {
            _subject.Close(false);

            _mockContextViewModel.Verify(cvm => cvm.ShowBaloon(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }
    }
}