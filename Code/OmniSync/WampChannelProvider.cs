namespace OmniSync
{
    using Newtonsoft.Json.Linq;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using SuperSocket.ClientEngine;
    using WampSharp.V1;
    using WebSocket4Net;

    public class WampChannelProvider : IWampChannelProvider
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly string _omniSyncUrl;

        private readonly IProxyConnectorFactory _proxyConnectorFactory;

        private readonly IWampChannelFactory<JToken> _wampChannelFactory;

        #endregion

        #region Constructors and Destructors

        public WampChannelProvider(
            IConfigurationService configurationService,
            IWampChannelFactory<JToken> wampChannelFactory,
            IProxyConnectorFactory proxyConnectorFactory)
        {
            _wampChannelFactory = wampChannelFactory;
            _proxyConnectorFactory = proxyConnectorFactory;
            _configurationService = configurationService;
            _omniSyncUrl = configurationService[ConfigurationProperties.OmniSyncUrl];
        }

        #endregion

        #region Public Methods and Operators

        public IWampChannel<JToken> GetChannel()
        {
            var webSocket = new WebSocket(_omniSyncUrl, "wamp", WebSocketVersion.None) { Proxy = CreateProxy() };
            return _wampChannelFactory.CreateChannel(webSocket);
        }

        #endregion

        #region Methods

        private IProxyConnector CreateProxy()
        {
            return _proxyConnectorFactory.Create(_configurationService.ProxyConfiguration);
        }

        #endregion
    }
}