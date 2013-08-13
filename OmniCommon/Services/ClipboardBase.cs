namespace OmniCommon.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OmniCommon.Interfaces;

    public abstract class ClipboardBase : IClipboard
    {
        private readonly List<ICanReceiveData> _dataReceivers;

        protected ClipboardBase()
        {
            _dataReceivers = new List<ICanReceiveData>();
        }

        public abstract void Dispose();

        public abstract Task<bool> Initialize();

        public abstract void PutData(string data);

        public void AddDataReceiver(ICanReceiveData dataReceiver)
        {
            if (!_dataReceivers.Contains(dataReceiver))
            {
                _dataReceivers.Add(dataReceiver);
            }
        }

        public void RemoveDataReceive(ICanReceiveData dataReceiver)
        {
            if (_dataReceivers.Contains(dataReceiver))
            {
                _dataReceivers.Remove(dataReceiver);
            }
        }

        protected virtual void NotifyReceivers(IClipboardData clipboardData)
        {
            foreach (var dataReceiver in _dataReceivers)
            {
                dataReceiver.DataReceived(clipboardData);
            }
        }
    }
}