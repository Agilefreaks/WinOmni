namespace PubNubClipboard.Api
{
    public interface IOmniApi
    {
        void SaveClippingAsync(string data, ISaveClippingCompleteHandler saveClippingCompleteHandler);
    }
}