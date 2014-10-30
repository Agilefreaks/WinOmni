namespace OmniDebug
{
    using OmniSync;

    public class WebsocketConnectionFactoryWrapper : WebsocketConnectionFactory
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        #endregion

        #region Public Methods and Operators

        public WebsocketConnectionFactoryWrapper(IWampChannelProvider wampChannelProvider)
            : base(wampChannelProvider)
        {
        }

        public override IWebsocketConnection Create()
        {
            return new WebsocketConnectionWrapper(GetWampChannel());
        }

        #endregion
    }
}