namespace OmnipasteTests.Services
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon;
    using OmniCommon.DataProviders;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;

    [TestFixture]
    public class ConfigurationServiceTests
    {
        private Mock<IConfigurationProvider> _mockConfigurationProvider;

        private IConfigurationService _subject;

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationProvider = new Mock<IConfigurationProvider>();
            _subject = new ConfigurationService(_mockConfigurationProvider.Object);
        }

        [Test]
        public void SaveAuthSettings_SetsAllSettingsNeededForAuthentication()
        {
            _subject.SaveAuthSettings("token", "refresh token");

            _mockConfigurationProvider.Verify(cp => cp.SetValue(ConfigurationProperties.AccessToken, "token"));
            _mockConfigurationProvider.Verify(cp => cp.SetValue(ConfigurationProperties.RefreshToken, "refresh token"));
        }

        [Test]
        public void ResetAuthSettings_ResetsAllSettingsNeededForAuthentication()
        {
            _subject.ResetAuthSettings();

            _mockConfigurationProvider.Verify(cp => cp.SetValue(ConfigurationProperties.AccessToken, string.Empty));
            _mockConfigurationProvider.Verify(cp => cp.SetValue(ConfigurationProperties.RefreshToken, string.Empty));
        }

        [Test]
        public void GetDeviceIdetifier_WhenTereIsAnExistingIdentifierSaved_ReturnsTheExistingIdentifier()
        {
            _mockConfigurationProvider.Setup(cp => cp.GetValue("DeviceIdentifier")).Returns("123456");

            var deviceIdentifier = _subject.DeviceIdentifier;

            deviceIdentifier.Should().Be("123456");
        }

        [Test]
        public void GetDeviceIdentifier_WhenThereIsntAnIdentifierSaved_ReturnsNewIdentifier()
        {
            _mockConfigurationProvider.Setup(cp => cp.GetValue("DeviceIdentifier")).Returns("");

            var deviceIdentifier = _subject.DeviceIdentifier;

            deviceIdentifier.Should().NotBe("");
        }

        [Test]
        public void GetDeviceIdentifier_WhenTHereIsntAnIdentifierSaved_SavesTheNewlyGeneratedIdentifier()
        {
            _mockConfigurationProvider.Setup(cp => cp.GetValue("DeviceIdentifier")).Returns("");

            var deviceIdentifier = _subject.DeviceIdentifier;

            _mockConfigurationProvider.Verify(cp => cp.SetValue("DeviceIdentifier", deviceIdentifier), Times.Once);
        }
    }
}