namespace Clipboard.Handlers.WindowsClipboard
{
    using System;

    public interface IWindowsClipboardWrapper
    {
        event EventHandler<ClipboardEventArgs> DataReceived;

        void SetData(string data);

        void StartWatchingClipboard();

        void StopWatchingClipboard();
    }
}