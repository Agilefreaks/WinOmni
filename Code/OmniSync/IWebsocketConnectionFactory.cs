namespace OmniSync
{
    public interface IWebsocketConnectionFactory
    {
        IWebsocketConnection Create();
    }
}