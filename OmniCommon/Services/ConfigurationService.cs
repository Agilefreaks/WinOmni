namespace OmniCommon.Services
{
    using System.Configuration;
    using OmniCommon.DataProviders;
    using OmniCommon.Interfaces;

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationProvider _configurationProvider;
        
        public CommunicationSettings CommunicationSettings { get; private set; }

        public string this[string key]
        {
            get
            {
                return ConfigurationManager.AppSettings[key];
            }
        }

        public ConfigurationService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void UpdateCommunicationChannel(string channel)
        {
            _configurationProvider.SetValue("channel", channel);
            Initialize();
        }

        public string GetCommunicationChannel()
        {
            return CommunicationSettings.Channel;
        }

        public void Initialize()
        {
            CommunicationSettings = new CommunicationSettings
                {
                    Channel = _configurationProvider.GetValue("channel")
                };
        }
    }
}