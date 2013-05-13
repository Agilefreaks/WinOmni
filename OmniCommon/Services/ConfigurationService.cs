using OmniCommon.Interfaces;

namespace OmniCommon.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public CommunicationSettings CommunicationSettings { get; private set; }

        public ConfigurationService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void Startup()
        {
            LoadCommunicationSettings();
        }

        public void UpdateCommunicationChannel(string channel)
        {
            _configurationProvider.SetValue("channel", channel);
            LoadCommunicationSettings();
        }

        public void LoadCommunicationSettings()
        {
            CommunicationSettings = new CommunicationSettings
                {
                    Channel = _configurationProvider.GetValue("channel")
                };
        }
    }
}