namespace OmnipasteTests.ViewModels
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Configuration;
    using Omnipaste.Dialog;
    using Omnipaste.Loading;
    using Omnipaste.Shell;
    using Omnipaste.UserToken;
    using OmniSync;

    [TestFixture]
    public class ShellViewModelTests
    {
        private Mock<IDialogViewModel> _mockDialogViewModel;

        private Mock<IConfigurationViewModel> _configurationViewModel;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IUserTokenViewModel> _mockUserTokenViewModel;

        private IShellViewModel _subject;

        private IOmniSyncService _omniSyncService;

        [SetUp]
        public void SetUp()
        {
            var kernel = new MoqMockingKernel();

            _mockDialogViewModel = kernel.GetMock<IDialogViewModel>();
            _configurationViewModel = kernel.GetMock<IConfigurationViewModel>();
            _mockEventAggregator = kernel.GetMock<IEventAggregator>();
            _mockUserTokenViewModel = kernel.GetMock<IUserTokenViewModel>();

            _omniSyncService = kernel.Get<OmniSyncService>();

            _subject = new ShellViewModel(
                _configurationViewModel.Object,
                _mockUserTokenViewModel.Object)
                       {
                           LoadingViewModel = new Mock<ILoadingViewModel>().Object,
                           DialogViewModel = _mockDialogViewModel.Object
                       };
        }

    }
}   