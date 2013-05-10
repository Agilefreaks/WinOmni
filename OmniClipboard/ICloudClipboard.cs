using System;

namespace Omniclipboard
{
    public interface ICloudClipboard : IDisposable
    {
        event EventHandler<ClipboardEventArgs> DataReceived;

        bool IsInitialized { get; }

        bool Initialize();

        void Copy(string str);
    }
}
