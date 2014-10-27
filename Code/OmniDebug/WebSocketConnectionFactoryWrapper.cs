namespace OmniDebug
{
    using OmniSync;

    public class WebsocketConnectionFactoryWrapper : IWebsocketConnectionFactory
    {
        #region Fields

        private readonly IWebsocketConnectionFactory _websocketConnectionFactory;

        #endregion

        #region Constructors and Destructors

        public WebsocketConnectionFactoryWrapper(IWebsocketConnectionFactory websocketConnectionFactory)
        {
            _websocketConnectionFactory = websocketConnectionFactory;
        }

        #endregion

        #region Public Methods and Operators

        public IWebsocketConnection Create()
        {
            return new WebsocketConnectionWrapper(_websocketConnectionFactory.Create());
        }

        #endregion
    }
}