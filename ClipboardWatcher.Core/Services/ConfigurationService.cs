namespace ClipboardWatcher.Core.Services
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

        public void LoadCommunicationSettings()
        {
            CommunicationSettings = new CommunicationSettings
                {
                    Channel = _configurationProvider.GetValue("channel"),
                    PublishKey = _configurationProvider.GetValue("publish-key"),
                    SecretKey = _configurationProvider.GetValue("secret-key"),
                    SubscribeKey = _configurationProvider.GetValue("subscribe-key")
                };
        }
    }
}