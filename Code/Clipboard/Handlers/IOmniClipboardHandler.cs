namespace Clipboard.Handlers
{
    using Clipboard.Dto;
    using OmniCommon.Handlers;

    public interface IOmniClipboardHandler : IClipboard, IResourceHandler<ClippingDto>
    {
    }
}