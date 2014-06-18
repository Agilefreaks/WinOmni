namespace OmnipasteTests.Shell
{
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Configuration;
    using Omnipaste.Dialog;
    using Omnipaste.Shell;
    using Omnipaste.Shell.ContextMenu;
    using Omnipaste.UserToken;

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

            kernel.Bind<IShellViewModel>().To<ShellViewModel>();

            _subject = kernel.Get<IShellViewModel>();
        }

        #endregion
    }
}