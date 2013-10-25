namespace Omnipaste.OmniClipboard.Core.Api
{
    public interface ISaveClippingCompleteHandler
    {
        void SaveClippingSucceeded();

        void SaveClippingFailed(string reason);
    }
}