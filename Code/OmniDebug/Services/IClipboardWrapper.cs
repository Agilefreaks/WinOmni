namespace OmniDebug.Services
{
    using Clipboard.API.Resources.v1;
    using Clipboard.Models;

    public interface IClippingsWrapper : IClippings
    {
        void MockGet(string clippingId, Clipping clipping);
    }
}