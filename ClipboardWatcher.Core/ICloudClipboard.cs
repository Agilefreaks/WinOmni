using System;

namespace ClipboardWatcher.Core
{
    public interface ICloudClipboard : IDisposable
    {
        event EventHandler<ClipboardEventArgs> DataReceived;

        bool IsInitialized { get; }

        bool Initialize();

        void Copy(string str);
    }
}
