namespace OmniCommon.Services
{
    using OmniCommon.DataProviders;
    using OmniCommon.Interfaces;

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationProvider _configurationProvider;
        
        public CommunicationSettings CommunicationSettings { get; private set; }
        
        public ConfigurationService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void UpdateCommunicationChannel(string channel)
        {
            _configurationProvider.SetValue("channel", channel);
            Initialize();
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