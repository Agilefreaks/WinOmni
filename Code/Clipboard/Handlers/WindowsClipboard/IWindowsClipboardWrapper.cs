namespace Clipboard.Handlers.WindowsClipboard
{
    using System;

    public interface IWindowsClipboardWrapper : IObservable<ClipboardEventArgs>, IDisposable
    {
        event EventHandler<ClipboardEventArgs> DataReceived;

        bool IsWatchingClippings { get; }

        void SetData(string data);

        void Start();

        void Stop();
    }
}