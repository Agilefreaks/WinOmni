namespace OmniCommon.Services
{
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;
    using OmniCommon.Domain;

    public class OmniService : IOmniService
    {
        private Task<bool> _startTask;

        private OmniServiceStatusEnum _status;

        public ILocalClipboard LocalClipboard { get; private set; }

        public IOmniClipboard OmniClipboard { get; private set; }

        public IClippingRepository ClippingRepository { get; private set; }

        public IEventAggregator EventAggregator { get; private set; }

        public OmniServiceStatusEnum Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
                EventAggregator.Publish(new OmniServiceStatusChanged(_status));
            }
        }

        protected string LastData { get; set; }

        public OmniService(ILocalClipboard localClipboard, IOmniClipboard omniClipboard, IClippingRepository clippingRepository, IEventAggregator eventAggregator)
        {
            LocalClipboard = localClipboard;
            OmniClipboard = omniClipboard;
            ClippingRepository = clippingRepository;
            EventAggregator = eventAggregator;
        }

        public Task<bool> Start()
        {
            return _startTask ?? (_startTask = Task<bool>.Factory.StartNew(
                () =>
                {
                    var initializeLocalClipboardTask = LocalClipboard.Initialize();
                    var initializeOmniClipboardTask = OmniClipboard.Initialize();
                    Task.WaitAll(initializeLocalClipboardTask, initializeOmniClipboardTask);
                    var couldInitializeClipboards = initializeLocalClipboardTask.Result && initializeOmniClipboardTask.Result;
                    if (couldInitializeClipboards)
                    {
                        OmniClipboard.AddDataReceiver(this);
                        LocalClipboard.AddDataReceiver(this);
                    }

                    _startTask = null;
                    Status = OmniServiceStatusEnum.Online;
                    return couldInitializeClipboards;
                }));
        }

        public void Stop()
        {
            LocalClipboard.RemoveDataReceive(this);
            OmniClipboard.RemoveDataReceive(this);
            LocalClipboard.Dispose();
            OmniClipboard.Dispose();
            Status = OmniServiceStatusEnum.Offline;
        }

        public void DataReceived(IClipboardData clipboardData)
        {
            var sender = clipboardData.GetSender();
            var previousStatus = Status;

            if (sender == LocalClipboard)
            {
                Status = OmniServiceStatusEnum.Sending;
                ProcessClipboardEvent(clipboardData, LastData, OmniClipboard);
            }
            else
            {
                Status = OmniServiceStatusEnum.Receiving;
                ProcessClipboardEvent(clipboardData, LastData, LocalClipboard);
            }

            Status = previousStatus;
        }

        private void ProcessClipboardEvent(IClipboardData clipboardData, string oldData, IClipboard clipboardToSendTo)
        {
            var data = clipboardData.GetData();
            if (!data.Equals(oldData) && !data.IsNullOrWhiteSpace())
            {
                LastData = clipboardData.GetData();

                clipboardToSendTo.PutData(data);

                EventAggregator.Publish(clipboardData);

                ClippingRepository.Save(new Clipping(data));
            }
        }
    }
}