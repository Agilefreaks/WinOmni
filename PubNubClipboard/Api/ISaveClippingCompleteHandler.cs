namespace PubNubClipboard.Api
{
    public interface ISaveClippingCompleteHandler
    {
        void SaveClippingSucceeded();

        void SaveClippingFailed(string reason);
    }
}