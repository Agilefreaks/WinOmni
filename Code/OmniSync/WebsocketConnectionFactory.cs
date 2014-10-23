namespace OmniSync
{
    using System.Configuration;
    using Newtonsoft.Json.Linq;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using WampSharp;

    public class WebsocketConnectionFactory : IWebsocketConnectionFactory
    {
        #region Fields

        private readonly IWampChannelFactory<JToken> _wampChannelFactory;

        private readonly IConfigurationService _configurationService;

        #endregion

        #region Constructors and Destructors

        public WebsocketConnectionFactory(IWampChannelFactory<JToken> wampChannelFactory, IConfigurationService configurationService)
        {
            _wampChannelFactory = wampChannelFactory;
            _configurationService = configurationService;
        }

        #endregion

        #region Public Methods and Operators

        public IWebsocketConnection Create()
        {
            IWampChannel<JToken> channel =
                _wampChannelFactory.CreateChannel(_configurationService[ConfigurationProperties.OmniSyncUrl]);

            var websocketConnection = new WebsocketConnection(channel);
            
            return websocketConnection;
        }

        #endregion
    }
}