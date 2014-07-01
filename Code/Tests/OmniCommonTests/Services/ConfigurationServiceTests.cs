namespace OmniCommonTests.Services
{
    using Moq;
    using NUnit.Framework;
    using OmniCommon;
    using OmniCommon.DataProviders;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;

    [TestFixture]
    public class ConfigurationServiceTests
    {
        [Test]
        public void SaveAuthSettings_SetsAllSettingsNeededForAuthentication()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            IConfigurationService service = new ConfigurationService(mockConfigurationProvider.Object);

            service.SaveAuthSettings("token", "refresh token");
            
            mockConfigurationProvider.Verify(cp => cp.SetValue("accessToken", "token"));
            mockConfigurationProvider.Verify(cp => cp.SetValue("refreshToken", "refresh token"));
        }

        [Test]
        public void ResetAuthSettings_ResetsAllSettingsNeededForAuthentication()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            IConfigurationService service = new ConfigurationService(mockConfigurationProvider.Object);

            service.ResetAuthSettings();
            
            mockConfigurationProvider.Verify(cp => cp.SetValue(ConfigurationProperties.AccessToken, string.Empty));
            mockConfigurationProvider.Verify(cp => cp.SetValue(ConfigurationProperties.RefreshToken, string.Empty));
        }
    }
}