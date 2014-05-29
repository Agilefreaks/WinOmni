using System.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WampSharp;
using WampSharp.Auxiliary.Client;

namespace OmniSync
{
    public class WebsocketConnectionFactory : IWebsocketConnectionFactory
    {
        private readonly IWampChannelFactory<JToken> _wampChannelFactory;

        public WebsocketConnectionFactory(IWampChannelFactory<JToken> wampChannelFactory)
        {
            _wampChannelFactory = wampChannelFactory;
        }

        public async Task<IWebsocketConnection> Create(string websocketServerUri)
        {
            IWampChannel<JToken> channel = _wampChannelFactory.CreateChannel(ConfigurationManager.AppSettings["OmniSyncUrl"]);

            await channel.OpenAsync();

            var wampClientConnectionMonitor = (WampClientConnectionMonitor<JToken>)channel.GetMonitor();
            var websocketConnection = new WebsocketConnection(channel) { RegistrationId = wampClientConnectionMonitor.SessionId};

            return websocketConnection;
        }
    }
}