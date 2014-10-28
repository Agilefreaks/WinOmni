namespace OmniDebug.Services
{
    using Clipboard.API.Resources.v1;
    using Clipboard.Models;

    public interface IClippingsWrapper : IClippings
    {
        void MockLast(Clipping clipping);
    }
}