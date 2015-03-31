namespace OmniDebug.Services
{
    using Clipboard.API.Resources.v1;
    using Clipboard.Dto;

    public interface IClippingsWrapper : IClippings
    {
        void MockGet(string clippingId, ClippingDto clippingDto);
    }
}