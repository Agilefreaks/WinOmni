namespace OmniCommon.Services
{
    using System.Threading.Tasks;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;

    public class OmniService : IOmniService
    {
        private Task _startTask;

        public ILocalClipboard LocalClipboard { get; private set; }

        public IOmniClipboard OmniClipboard { get; private set; }

        protected string LastReceivedEventArgs { get; set; }

        protected string LastSentEventArgs { get; set; }

        public OmniService(ILocalClipboard localClipboard, IOmniClipboard omniClipboard)
        {
            LocalClipboard = localClipboard;
            OmniClipboard = omniClipboard;
        }

        public Task Start()
        {
            return _startTask ?? (_startTask = Task.Factory.StartNew(
                () =>
                {
                    Task.WaitAll(LocalClipboard.Initialize(), OmniClipboard.Initialize());
                    OmniClipboard.AddDataReceiver(this);
                    LocalClipboard.AddDataReceiver(this);
                    _startTask = null;
                }));
        }

        public void Stop()
        {
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
                clipboardToSendTo.SendData(data);
                sentData = data;
            }

            return sentData;
        }
    }
}