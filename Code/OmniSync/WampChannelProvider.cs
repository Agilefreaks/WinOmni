namespace OmniSync
{
    using Newtonsoft.Json.Linq;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using SuperSocket.ClientEngine;
    using WampSharp.V1;
    using WebSocket = WebSocket4Net.WebSocket;

    public class WampChannelProvider : IWampChannelProvider
    {
        private readonly IWampChannelFactory<JToken> _wampChannelFactory;

        private readonly string _omniSyncUrl;

        public WampChannelProvider(IConfigurationService configurationService, IWampChannelFactory<JToken> wampChannelFactory)
        {
            _wampChannelFactory = wampChannelFactory;
            _omniSyncUrl = configurationService[ConfigurationProperties.OmniSyncUrl];
        }

        public IWampChannel<JToken> GetChannel()
        {
            var webSocket = new WebSocket(_omniSyncUrl, "wamp") { Proxy = CreateProxy() };
            return _wampChannelFactory.CreateChannel(webSocket);
        }

        private IProxyConnector CreateProxy()
        {
            return null;
        }
    }
}