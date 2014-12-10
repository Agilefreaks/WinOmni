namespace OmnipasteTests.Services
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using OmniCommon.Settings;
    using Omnipaste.Services;

    [TestFixture]
    public class ConfigurationServiceTests
    {
        private Mock<IConfigurationContainer> _mockConfigurationProvider;

        private IConfigurationService _subject;

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationProvider = new Mock<IConfigurationContainer>();
            _subject = new ConfigurationService(_mockConfigurationProvider.Object);
        }

        [Test]
        public void SaveAuthSettings_SetsAllSettingsNeededForAuthentication()
        {
            _subject.SaveAuthSettings(new OmnipasteCredentials("token", "refresh token"));

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
        public void GetDeviceIdentifier_WhenTereIsAnExistingIdentifierSaved_ReturnsTheExistingIdentifier()
        {
            _mockConfigurationProvider.Setup(cp => cp.GetValue("DeviceIdentifier")).Returns("123456");

            var deviceIdentifier = _subject.DeviceIdentifier;

            deviceIdentifier.Should().Be("123456");
        }

        [Test]
        public void GetDeviceIdentifier_WhenThereIsNoIdentifierSaved_ReturnsNull()
        {
            _mockConfigurationProvider.Setup(cp => cp.GetValue("DeviceIdentifier")).Returns((string)null);

            var deviceIdentifier = _subject.DeviceIdentifier;

            deviceIdentifier.Should().Be(null);
        }

        [Test]
        public void GetProxyConfiguration_ConfigurationProviderDoesNotContainAProxyConfiguration_ReturnsEmptyProxyConfiguration()
        {
            _mockConfigurationProvider.Setup(x => x.GetValue(ConfigurationProperties.ProxyConfiguration))
                .Returns((string)null);

            var proxyConfiguration = _subject.ProxyConfiguration;

            proxyConfiguration.Should().Be(ProxyConfiguration.Empty());
        }

        [Test]
        public void GetProxyConfiguration_ConfigurationProviderHasInvalidData_ReturnsEmptyProxyConfiguration()
        {
            _mockConfigurationProvider.Setup(x => x.GetValue(ConfigurationProperties.ProxyConfiguration))
                .Returns("some-garbled-data");

            var proxyConfiguration = _subject.ProxyConfiguration;

            proxyConfiguration.Should().Be(ProxyConfiguration.Empty());
        }

        [Test]
        public void GetProxyConfiguration_Always_CanReadAConfigurationPreviouslySaved()
        {
            string valueSet = null;
            var proxyConfiguration = new ProxyConfiguration
                                         {
                                             Address = "testA",
                                             Password = "testP",
                                             Username = "testU",
                                             Port = 12,
                                             Type = ProxyTypeEnum.Socks4
                                         };
            _mockConfigurationProvider.Setup(
                x => x.SetValue(ConfigurationProperties.ProxyConfiguration, It.IsAny<string>()))
                .Callback<string, string>((key, value) => valueSet = value);

            _subject.SaveProxyConfiguration(proxyConfiguration);
            valueSet.Should().NotBeNull();
            _mockConfigurationProvider.Setup(x => x.GetValue(ConfigurationProperties.ProxyConfiguration)).Returns(valueSet);

            _subject.ProxyConfiguration.Should().Be(proxyConfiguration);
        }

        [Test]
        public void IsSMSSuffixEnabled_ConfigurationProviderHasAInvalidEntry_ReturnsTrue()
        {
            _mockConfigurationProvider.Setup(x => x.GetValue(ConfigurationProperties.SMSSuffixEnabled)).Returns("test");

            _subject.IsSMSSuffixEnabled.Should().BeTrue();
        }

        [Test]
        public void IsSMSSuffixEnabled_ConfigurationProviderHasAFalseEntryStored_ReturnsFalse()
        {
            _mockConfigurationProvider.Setup(x => x.GetValue(ConfigurationProperties.SMSSuffixEnabled)).Returns("false");

            _subject.IsSMSSuffixEnabled.Should().BeFalse();
        }

        [Test]
        public void IsSMSSuffixEnabled_ConfigurationProviderHasATrueEntryStored_ReturnsTrue()
        {
            _mockConfigurationProvider.Setup(x => x.GetValue(ConfigurationProperties.SMSSuffixEnabled)).Returns("true");

            _subject.IsSMSSuffixEnabled.Should().BeTrue();
        }

        [Test]
        public void SetIsSMSSuffixEnabled_Always_SetsItAsTextOnTheConfigurationProvider()
        {
            _subject.IsSMSSuffixEnabled = true;

            _mockConfigurationProvider.Verify(x => x.SetValue(ConfigurationProperties.SMSSuffixEnabled, "True"));
        }

        [Test]
        public void SetDeviceIdentifier_Always_SavesTheGivenValue()
        {
            _subject.DeviceIdentifier = "someId";

            _mockConfigurationProvider.Verify(x => x.SetValue(ConfigurationProperties.DeviceIdentifier, "someId"));
        }

        [Test]
        public void IsNewDevice_NoDeviceIdentifierIsStored_ReturnsTrue()
        {
            _mockConfigurationProvider.Setup(x => x.GetValue(ConfigurationProperties.DeviceIdentifier))
                .Returns(string.Empty);

            _subject.IsNewDevice.Should().BeTrue();
        }

        [Test]
        public void IsNewDevice_ADeviceIdentifierIsStoredAndNoDeviceIdentifierChangesHaveOccured_ReturnsFalse()
        {
            _mockConfigurationProvider.Setup(x => x.GetValue(ConfigurationProperties.DeviceIdentifier))
                .Returns("someId");

            _subject.IsNewDevice.Should().BeFalse();
        }

        [Test]
        public void IsNewDevice_ADeviceIdentifierIsStoredButACallToSaveANewDeviceIdentifierHasBeenMade_ReturnsTrue()
        {
            _mockConfigurationProvider.Setup(x => x.GetValue(ConfigurationProperties.DeviceIdentifier))
                .Returns("someId");
            _subject.DeviceIdentifier = "someNewId";

            _subject.IsNewDevice.Should().BeTrue();
        }
    }
}