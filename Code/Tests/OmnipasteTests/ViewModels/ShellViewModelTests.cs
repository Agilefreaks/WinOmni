namespace OmnipasteTests.ViewModels
{
    using Caliburn.Micro;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Configuration;
    using Omnipaste.Dialog;
    using Omnipaste.Framework;
    using Omnipaste.Loading;
    using Omnipaste.Shell;
    using Omnipaste.UserToken;

    [TestFixture]
    public class ShellViewModelTests
    {
        private Mock<IDialogViewModel> _mockDialogViewModel;

        private Mock<IConfigurationViewModel> _configurationViewModel;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IUserTokenViewModel> _mockUserTokenViewModel;

        private IShellViewModel _subject;

        [SetUp]
        public void SetUp()
        {
            _mockDialogViewModel = new Mock<IDialogViewModel>();
            _configurationViewModel = new Mock<IConfigurationViewModel>();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockUserTokenViewModel = new Mock<IUserTokenViewModel>();

            _subject = new ShellViewModel(
                _configurationViewModel.Object,
                _mockEventAggregator.Object,
                _mockUserTokenViewModel.Object,
                new Mock<IDialogService>().Object)
                       {
                           LoadingViewModel = new Mock<ILoadingViewModel>().Object,
                           DialogViewModel = _mockDialogViewModel.Object
                       };
        }
    }
}   