namespace Omnipaste.Services
{
    using OmniCommon.Interfaces;
    using OmniCommon.Services;

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public CommunicationSettings CommunicationSettings { get; private set; }

        public ConfigurationService(IConfigurationProvider configurationProvider)
        {
            this._configurationProvider = configurationProvider;
        }

        public void Startup()
        {
            this.LoadCommunicationSettings();
        }

        public void UpdateCommunicationChannel(string channel)
        {
            this._configurationProvider.SetValue("channel", channel);
            this.LoadCommunicationSettings();
        }

        public void LoadCommunicationSettings()
        {
            this.CommunicationSettings = new CommunicationSettings
                {
                    Channel = this._configurationProvider.GetValue("channel")
                };
        }
    }
}