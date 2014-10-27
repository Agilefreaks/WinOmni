namespace OmniDebug
{
    using Newtonsoft.Json.Linq;
    using OmniCommon.Interfaces;
    using OmniSync;
    using WampSharp;


    public class WebsocketConnectionFactoryWrapper : WebsocketConnectionFactory
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        #endregion

        #region Public Methods and Operators

        public WebsocketConnectionFactoryWrapper(IWampChannelFactory<JToken> wampChannelFactory, IConfigurationService configurationService)
            : base(wampChannelFactory, configurationService)
        {
        }

        public override IWebsocketConnection Create()
        {
            return new WebsocketConnectionWrapper(GetWampChannel());
        }

        #endregion
    }
}