namespace OmniSync
{
    using Newtonsoft.Json.Linq;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using WampSharp;

    public class WebsocketConnectionFactory : IWebsocketConnectionFactory
    {
        #region Fields

        protected readonly IWampChannelFactory<JToken> WampChannelFactory;

        protected readonly IConfigurationService ConfigurationService;

        #endregion

        #region Constructors and Destructors

        public WebsocketConnectionFactory(IWampChannelFactory<JToken> wampChannelFactory, IConfigurationService configurationService)
        {
            WampChannelFactory = wampChannelFactory;
            ConfigurationService = configurationService;
        }

        #endregion

        #region Public Methods and Operators

        public virtual IWebsocketConnection Create()
        {
            return new WebsocketConnection(GetWampChannel());
        }

        protected IWampChannel<JToken> GetWampChannel()
        {
            return WampChannelFactory.CreateChannel(ConfigurationService[ConfigurationProperties.OmniSyncUrl]);
        }

        #endregion
    }
}