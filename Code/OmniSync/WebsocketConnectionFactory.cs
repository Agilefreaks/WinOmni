namespace OmniSync
{
    using Newtonsoft.Json.Linq;
    using WampSharp.V1;

    public class WebsocketConnectionFactory : IWebsocketConnectionFactory
    {
        private readonly IWampChannelProvider _wampChannelProvider;

        #region Constructors and Destructors

        public WebsocketConnectionFactory(IWampChannelProvider wampChannelProvider)
        {
            _wampChannelProvider = wampChannelProvider;
        }

        #endregion

        #region Public Methods and Operators

        public virtual IWebsocketConnection Create()
        {
            return new WebsocketConnection(GetWampChannel());
        }

        protected IWampChannel<JToken> GetWampChannel()
        {
            return _wampChannelProvider.GetChannel();
        }

        #endregion
    }
}