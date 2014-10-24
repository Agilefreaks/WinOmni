namespace OmniDebug
{
    using OmniSync;

    public class WebsocketConnectionFactoryWrapper : IWebsocketConnectionFactory
    {
        private readonly IWebsocketConnectionFactory _websocketConnectionFactory;

        public WebsocketConnectionFactoryWrapper(IWebsocketConnectionFactory websocketConnectionFactory)
        {
            _websocketConnectionFactory = websocketConnectionFactory;
        }

        public IWebsocketConnection Create()
        {
            return new WebsocketConnectionWrapper(_websocketConnectionFactory.Create());
        }
    }
}