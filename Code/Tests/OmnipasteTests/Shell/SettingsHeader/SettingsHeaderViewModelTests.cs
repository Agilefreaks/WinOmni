namespace OmnipasteTests.Shell.SettingsHeader
{
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Shell.Settings;
    using Omnipaste.Shell.SettingsHeader;

    [TestFixture]
    public class SettingsHeaderViewModelTests
    {
        private ISettingsHeaderViewModel _settingsHeaderViewModel;

        private Mock<ISettingsViewModel> _mockSettingsViewModel;

        [SetUp]
        public void SetUp()
        {
            _mockSettingsViewModel = new Mock<ISettingsViewModel>();
            _settingsHeaderViewModel = new SettingsHeaderViewModel { SettingsViewModel = _mockSettingsViewModel.Object };
        }

        [Test]
        public void ToggleSettingsFlyout_ChangesSettingsViewModelIsOpenProperty()
        {
            _settingsHeaderViewModel.ToggleSettingsFlyout();

            _mockSettingsViewModel.VerifySet(svm => svm.IsOpen = true, Times.Once());
        }

        [Test]
        public void ToggleSettingsFlyout_WhenSettingsIsOpen_WillCloseTheFlyout()
        {
            _mockSettingsViewModel.SetupGet(svm => svm.IsOpen).Returns(true);

            _settingsHeaderViewModel.ToggleSettingsFlyout();

            _mockSettingsViewModel.VerifySet(svm => svm.IsOpen = false, Times.Once());
        }
    }
}