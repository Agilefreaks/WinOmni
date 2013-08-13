namespace OmniCommon.Services
{
    using OmniCommon.DataProviders;
    using OmniCommon.Interfaces;

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IAppConfigurationProvider _appConfigurationProvider;

        public CommunicationSettings CommunicationSettings { get; private set; }
        
        public ApiConfig ApiConfig { get; set; }

        public ConfigurationService(IConfigurationProvider configurationProvider, IAppConfigurationProvider appConfigurationProvider)
        {
            _configurationProvider = configurationProvider;
            _appConfigurationProvider = appConfigurationProvider;
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

            ApiConfig = new ApiConfig
                {
                    BaseUrl = _appConfigurationProvider.GetValue("apiUrl"),
                    Resources = new Resources
                        {
                            Clippings = _appConfigurationProvider.GetValue("clippingResource")
                        }
                };
        }

    }
}