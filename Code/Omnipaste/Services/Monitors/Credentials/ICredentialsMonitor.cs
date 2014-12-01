namespace Omnipaste.Services.Monitors.Credentials
{
    using OmniCommon.Settings;
    using Omnipaste.Services.Monitors.SettingsMonitor;

    public interface ICredentialsMonitor : ISettingsMonitor<OmnipasteCredentials>
    {
    }
}