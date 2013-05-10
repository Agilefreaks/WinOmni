using System;

namespace Omniclipboard
{
    public interface IOmniclipboard : IDisposable
    {
        event EventHandler<ClipboardEventArgs> DataReceived;

        bool IsInitialized { get; }

        string Channel { get; }

        bool Initialize();

        void Copy(string str);
    }
}
