using System;

namespace ClipboardWatcher.Core
{
    public interface ICloudClipboard
    {
        event EventHandler<ClipboardEventArgs> DataReceived;

        void Copy(string str);
    }
}
