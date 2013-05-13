using System;

namespace OmniCommon.Interfaces
{
    public interface IClipboard : IDisposable
    {
        event EventHandler<ClipboardEventArgs> DataReceived;

        bool Initialize();

        void SendData(string data);
    }
}