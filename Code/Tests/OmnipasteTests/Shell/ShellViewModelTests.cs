namespace OmnipasteTests.Shell
{
    using Caliburn.Micro;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Configuration;
    using Omnipaste.Dialog;
    using Omnipaste.Loading;
    using Omnipaste.Shell;
    using Omnipaste.Shell.ContextMenu;
    using Omnipaste.UserToken;
    using OmniSync;

    [TestFixture]
    public class ShellViewModelTests
    {
        #region Fields

        private Mock<IConfigurationViewModel> _configurationViewModel;

        private Mock<IContextMenuViewModel> _mockContextViewModel;

        private Mock<IDialogViewModel> _mockDialogViewModel;

        private Mock<IUserTokenViewModel> _mockUserTokenViewModel;

        private IShellViewModel _subject;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void SetUp()
        {
            var kernel = new MoqMockingKernel();

            _mockDialogViewModel = kernel.GetMock<IDialogViewModel>();
            _configurationViewModel = kernel.GetMock<IConfigurationViewModel>();
            _mockUserTokenViewModel = kernel.GetMock<IUserTokenViewModel>();
            _mockContextViewModel = kernel.GetMock<IContextMenuViewModel>();

            _subject = new ShellViewModel(
                _configurationViewModel.Object,
                _mockUserTokenViewModel.Object,
                _mockContextViewModel.Object)
                           {
                               LoadingViewModel = new Mock<ILoadingViewModel>().Object,
                               DialogViewModel = _mockDialogViewModel.Object
                           };
        }

        #endregion
    }
}