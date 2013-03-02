using System;

namespace ClipboardWatcher.Core
{
    public interface ICloudClipboard : IDisposable
    {
        event EventHandler<ClipboardEventArgs> DataReceived;

        void Copy(string str);
    }
}
