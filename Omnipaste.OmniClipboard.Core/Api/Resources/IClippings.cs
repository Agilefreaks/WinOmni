namespace Omnipaste.OmniClipboard.Core.Api.Resources
{
    public interface IClippings
    {
        void SaveAsync(string data, ISaveClippingCompleteHandler handler);

        void GetLastAsync(IGetClippingCompleteHandler handler);
    }
}
