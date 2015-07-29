namespace OmnipasteTests.Profile.UserProfile
{
    using System.Reflection.Emit;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Profile.UserProfile;

    [TestFixture]
    public class UserProfileViewModelTests
    {
        [SetUp]
        public void SetUp()
        {
            _mockIConfigurationService = new Mock<IConfigurationService>();
            _mockDevicesApi = new Mock<IDevices>();
        }

        private Mock<IConfigurationService> _mockIConfigurationService;

        private Mock<IDevices> _mockDevicesApi;

        [Test]
        public void Constructor_Always_SetsUser()
        {
            _mockIConfigurationService.SetupGet(mock => mock.UserInfo).Returns(new UserInfo());

            var userProfileViewModel = new UserProfileViewModel(_mockIConfigurationService.Object, _mockDevicesApi.Object);

            // Todo: add the verification thath the user is not null
        }

        [Test]
        public void Constructor_Always_SetsDevices()
        {
            _mockIConfigurationService.SetupGet(mock => mock.UserInfo).Returns(new UserInfo());

            var userProfileViewModel = new UserProfileViewModel(_mockIConfigurationService.Object, _mockDevicesApi.Object);

            userProfileViewModel.Devices.Should().NotBeNull();
        }
    }
}