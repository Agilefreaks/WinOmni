using System;
using OmniCommon;

namespace PubNubClipboard
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
