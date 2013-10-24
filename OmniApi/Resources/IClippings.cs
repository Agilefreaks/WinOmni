namespace OmniApi.Resources
{
    using Omnipaste.OmniClipboard.Core.Api;

    public interface IClippings
    {
        void SaveAsync(string data, ISaveClippingCompleteHandler handler);

        void GetLastAsync(IGetClippingCompleteHandler handler);
    }
}