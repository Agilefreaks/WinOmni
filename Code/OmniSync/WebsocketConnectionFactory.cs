namespace OmniSync
{
    using System.Configuration;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using WampSharp;
    using WampSharp.Auxiliary.Client;

    public class WebsocketConnectionFactory : IWebsocketConnectionFactory
    {
        #region Fields

        private readonly IWampChannelFactory<JToken> _wampChannelFactory;

        #endregion

        #region Constructors and Destructors

        public WebsocketConnectionFactory(IWampChannelFactory<JToken> wampChannelFactory)
        {
            _wampChannelFactory = wampChannelFactory;
        }

        #endregion

        #region Public Methods and Operators

        public async Task<IWebsocketConnection> Create(string websocketServerUri)
        {
            IWampChannel<JToken> channel =
                _wampChannelFactory.CreateChannel(ConfigurationManager.AppSettings["OmniSyncUrl"]);
            await channel.OpenAsync();

            var wampClientConnectionMonitor = (WampClientConnectionMonitor<JToken>)channel.GetMonitor();
            var websocketConnection = new WebsocketConnection(channel)
                                      {
                                          RegistrationId =
                                              wampClientConnectionMonitor.SessionId
                                      };
            
            return websocketConnection;
        }

        #endregion
    }
}