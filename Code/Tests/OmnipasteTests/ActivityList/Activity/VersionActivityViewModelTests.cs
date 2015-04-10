namespace OmnipasteTests.ActivityList.Activity
{
    using Moq;
    using NUnit.Framework;
    using Omnipaste.ActivityList.Activity;
    using Omnipaste.Services;

    [TestFixture]
    public class VersionActivityViewModelTests
    {
        private VersionActivityViewModel _subject;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private Mock<IUpdaterService> _mockUpdaterService;

        private Mock<ISessionManager> _mockSessionManager;

        [SetUp]
        public void SetUp()
        {
            _mockUiRefreshService = new Mock<IUiRefreshService>();
            _mockUpdaterService = new Mock<IUpdaterService>();
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };

            _subject = new VersionActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object, _mockUpdaterService.Object);
        }

        [Test]
        public void UpdateApp_Always_InstallsNewVersion()
        {
            _subject.UpdateApp();

            _mockUpdaterService.Verify(m => m.InstallNewVersion(It.IsAny<bool>()));
        }
    }
}
