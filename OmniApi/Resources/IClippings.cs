namespace OmniApi.Resources
{
    using Omnipaste.OmniClipboard.Core.Api;

    public interface IClippings
    {
        string Channel { get; set; }

        void SaveAsync(string data, ISaveClippingCompleteHandler handler);

        void GetLastAsync(IGetClippingCompleteHandler handler);
    }
}