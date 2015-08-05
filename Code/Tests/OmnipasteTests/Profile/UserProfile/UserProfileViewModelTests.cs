namespace OmnipasteTests.Profile.UserProfile
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omni;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Profile.UserProfile;

    [TestFixture]
    public class UserProfileViewModelTests
    {
        [SetUp]
        public void SetUp()
        {
            _mockIConfigurationService = new Mock<IConfigurationService>();
            _mockDevicesApi = new Mock<IDevices>();
            _mockOmniService = new Mock<IOmniService>();
        }

        private Mock<IConfigurationService> _mockIConfigurationService;

        private Mock<IDevices> _mockDevicesApi;

        private Mock<IOmniService> _mockOmniService;

        [Test]
        public void Constructor_Always_SetsDevices()
        {
            _mockIConfigurationService.SetupGet(mock => mock.UserInfo).Returns(new UserInfo());

            var userProfileViewModel = new UserProfileViewModel(
                _mockIConfigurationService.Object,
                _mockDevicesApi.Object,
                _mockOmniService.Object);

            userProfileViewModel.Devices.Should().NotBeNull();
        }

        [Test]
        public void Constructor_Always_SetsUser()
        {
            _mockIConfigurationService.SetupGet(mock => mock.UserInfo).Returns(new UserInfo());

            var userProfileViewModel = new UserProfileViewModel(
                _mockIConfigurationService.Object,
                _mockDevicesApi.Object,
                _mockOmniService.Object);

            userProfileViewModel.User.Should().NotBeNull();
        }

        [Test]
        public void Identifier_Always_ProxiesToUserIdentifier()
        {
            var userProfileViewModel = new UserProfileViewModel(
                _mockIConfigurationService.Object,
                _mockDevicesApi.Object,
                _mockOmniService.Object);
             
            userProfileViewModel.User = new ContactModel(new ContactEntity { FirstName = "Ionica", LastName = "din Deal" });

            userProfileViewModel.Identifier.Should().Be("Ionica din Deal");
        }
    }
}