using System;

namespace OmniCommon.Interfaces
{
    using System.Threading.Tasks;

    public interface IClipboard : IDisposable
    {
        void AddDataReceiver(ICanReceiveData dataReceiver);

        void RemoveDataReceive(ICanReceiveData dataReceiver);

        Task<bool> Initialize();

        void SendData(string data);
    }
}