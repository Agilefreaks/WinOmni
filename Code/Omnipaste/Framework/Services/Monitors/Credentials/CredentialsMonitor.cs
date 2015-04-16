namespace Omnipaste.Framework.Services.Monitors.Credentials
{
    using OmniCommon;
    using OmniCommon.Interfaces;
    using OmniCommon.Settings;
    using Omnipaste.Framework.Services.Monitors.SettingsMonitor;

    public class CredentialsMonitor : SettingsMonitorBase<OmnipasteCredentials>, ICredentialsMonitor
    {
        public CredentialsMonitor(IConfigurationService configurationService)
            : base(configurationService, ConfigurationProperties.OmnipasteCredentials)
        {
        }
    }
}