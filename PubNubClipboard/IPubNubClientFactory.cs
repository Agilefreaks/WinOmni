namespace PubNubClipboard
{
    using PubNubWrapper;

    public interface IPubNubClientFactory
    {
        IPubNubClient Create();
    }
}