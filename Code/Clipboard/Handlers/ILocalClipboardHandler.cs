namespace Clipboard.Handlers
{
    using System;
    using Clipboard.Handlers.WindowsClipboard;

    public interface ILocalClipboardHandler : IClipboard, IObserver<ClipboardEventArgs>
    {
    }
}