using System;

namespace OmniCommon.Interfaces
{
    using System.Threading.Tasks;

    public interface IClipboard : IDisposable
    {
        event EventHandler<ClipboardEventArgs> DataReceived;

        Task<bool> Initialize();

        void SendData(string data);
    }
}