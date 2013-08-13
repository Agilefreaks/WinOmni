namespace Omnipaste.OmniClipboard.Core.Api
{
    public interface IOmniApi
    {
        void SaveClippingAsync(string data, ISaveClippingCompleteHandler handler);

        void GetLastClippingAsync(IGetClippingCompleteHandler handler);
    }
}