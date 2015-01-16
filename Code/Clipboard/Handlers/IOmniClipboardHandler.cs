namespace Clipboard.Handlers
{
    using Clipboard.Models;
    using OmniCommon.Handlers;

    public interface IOmniClipboardHandler : IClipboard, IResourceHandler<Clipping>
    {
    }
}