namespace OmniCommon.Services
{
    using System.Threading.Tasks;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;

    public class OmniService : IOmniService
    {
        private Task<bool> _startTask;

        public ILocalClipboard LocalClipboard { get; private set; }

        public IOmniClipboard OmniClipboard { get; private set; }

        protected string LastReceivedEventArgs { get; set; }

        protected string LastSentEventArgs { get; set; }

        public OmniService(ILocalClipboard localClipboard, IOmniClipboard omniClipboard)
        {
            LocalClipboard = localClipboard;
            OmniClipboard = omniClipboard;
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

                    return couldInitializeClipboards;
                }));
        }

        public void Stop()
        {
            LocalClipboard.RemoveDataReceive(this);
            OmniClipboard.RemoveDataReceive(this);
            LocalClipboard.Dispose();
            OmniClipboard.Dispose();
        }

        public void DataReceived(IClipboardData clipboardData)
        {
            var sender = clipboardData.GetSender();
            if (sender == LocalClipboard)
            {
                LastSentEventArgs = ProcessClipboardEvent(clipboardData, LastReceivedEventArgs, OmniClipboard);
            }
            else
            {
                LastReceivedEventArgs = ProcessClipboardEvent(clipboardData, LastSentEventArgs, LocalClipboard);
            }
        }

        private static string ProcessClipboardEvent(
            IClipboardData clipboardData, string oldData, IClipboard clipboardToSendTo)
        {
            string sentData = null;
            var data = clipboardData.GetData();
            if (!data.Equals(oldData) && !data.IsNullOrWhiteSpace())
            {
                clipboardToSendTo.PutData(data);
                sentData = data;
            }

            return sentData;
        }
    }
}