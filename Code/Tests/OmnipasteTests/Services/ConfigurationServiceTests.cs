namespace OmnipasteTests.Services
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon;
    using OmniCommon.DataProviders;
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

        [Test]
        public void GetProxyConfiguration_ConfigurationProviderDoesNotContainAProxyConfiguration_ReturnsEmptyProxyConfiguration()
        {
            _mockConfigurationProvider.SetupGet(x => x[ConfigurationProperties.ProxyConfiguration])
                .Returns((string)null);

            var proxyConfiguration = _subject.ProxyConfiguration;

            proxyConfiguration.Should().Be(ProxyConfiguration.Empty());
        }

        [Test]
        public void GetProxyConfiguration_ConfigurationProviderHasInvalidData_ReturnsEmptyProxyConfiguration()
        {
            _mockConfigurationProvider.SetupGet(x => x[ConfigurationProperties.ProxyConfiguration])
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
            _mockConfigurationProvider.SetupGet(x => x[ConfigurationProperties.SMSSuffixEnabled]).Returns("test");

            _subject.IsSMSSuffixEnabled.Should().BeTrue();
        }

        [Test]
        public void IsSMSSuffixEnabled_ConfigurationProviderHasAFalseEntryStored_ReturnsFalse()
        {
            _mockConfigurationProvider.SetupGet(x => x[ConfigurationProperties.SMSSuffixEnabled]).Returns("false");

            _subject.IsSMSSuffixEnabled.Should().BeFalse();
        }

        [Test]
        public void IsSMSSuffixEnabled_ConfigurationProviderHasATrueEntryStored_ReturnsTrue()
        {
            _mockConfigurationProvider.SetupGet(x => x[ConfigurationProperties.SMSSuffixEnabled]).Returns("true");

            _subject.IsSMSSuffixEnabled.Should().BeTrue();
        }

        [Test]
        public void SetIsSMSSuffixEnabled_Always_SetsItAsTextOnTheConfigurationProvider()
        {
            _subject.IsSMSSuffixEnabled = true;

            _mockConfigurationProvider.VerifySet(x => x[ConfigurationProperties.SMSSuffixEnabled] = "True");
        }
    }
}