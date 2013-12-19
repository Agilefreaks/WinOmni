namespace OmniCommon.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OmniCommon.Interfaces;

    public abstract class ClipboardBase : IClipboard
    {
        private readonly List<IDataReceiver> _dataReceivers;

        protected ClipboardBase()
        {
            _dataReceivers = new List<IDataReceiver>();
        }

        public abstract void Dispose();

        public abstract Task<bool> Initialize();    

        public abstract void PutData(string data);

        public void AddDataReceiver(IDataReceiver dataReceiver)
        {
            if (!_dataReceivers.Contains(dataReceiver))
            {
                _dataReceivers.Add(dataReceiver);
            }
        }

        public void RemoveDataReceive(IDataReceiver dataReceiver)
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