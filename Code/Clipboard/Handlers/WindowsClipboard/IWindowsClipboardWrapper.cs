namespace Clipboard.Handlers.WindowsClipboard
{
    using System;

    public interface IWindowsClipboardWrapper
    {
        event EventHandler<ClipboardEventArgs> DataReceived;

        bool IsWatchingClippings { get; }

        void SetData(string data);

        void StartWatchingClipboard();

        void StopWatchingClipboard();
    }
}