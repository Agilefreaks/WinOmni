namespace Omnipaste.Services.Monitors.ProxyConfiguration
{
    using OmniCommon;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Services.Monitors.SettingsMonitor;

    public class ProxyConfigurationMonitor : SettingsMonitorBase<ProxyConfiguration>, IProxyConfigurationMonitor
    {
        public ProxyConfigurationMonitor(IConfigurationService configurationService)
            : base(configurationService, ConfigurationProperties.ProxyConfiguration)
        {
        }
    }
}