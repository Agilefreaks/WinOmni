using System;

namespace OmniCommon.Interfaces
{
    using System.Threading.Tasks;

    public interface IClipboard : IDisposable
    {
        void AddDataReceiver(IDataReceiver dataReceiver);

        void RemoveDataReceive(IDataReceiver dataReceiver);

        Task<bool> Initialize();

        void PutData(string data);
    }
}