using OmniCommon.ExtensionMethods;
using OmniCommon.Interfaces;

namespace OmniCommon.Services
{
    public class OmniService : IOmniService
    {
        public ILocalClipboard LocalClipboard { get; private set; }

        public IOmniClipboard OmniClipboard { get; private set; }

        protected bool CanProcessData { get; set; }

        protected ClipboardEventArgs LastReceivedEventArgs { get; set; }

        protected ClipboardEventArgs LastSentEventArgs { get; set; }

        public OmniService(ILocalClipboard localClipboard, IOmniClipboard omniClipboard)
        {
            LocalClipboard = localClipboard;
            OmniClipboard = omniClipboard;
            LocalClipboard.DataReceived += LocalClipboardOnDataReceived;
            OmniClipboard.DataReceived += OmniClipboardOnDataReceived;
        }

        public void Start()
        {
            LocalClipboard.Initialize();
            OmniClipboard.Initialize();
            CanProcessData = true;
        }

        public void Stop()
        {
            CanProcessData = false;
            LocalClipboard.Dispose();
            OmniClipboard.Dispose();
        }

        private void LocalClipboardOnDataReceived(object sender, ClipboardEventArgs clipboardEventArgs)
        {
            LastSentEventArgs = ProcessClipboardEvent(clipboardEventArgs, LastReceivedEventArgs, OmniClipboard);
        }

        private void OmniClipboardOnDataReceived(object sender, ClipboardEventArgs clipboardEventArgs)
        {
            LastReceivedEventArgs = ProcessClipboardEvent(clipboardEventArgs, LastSentEventArgs, LocalClipboard);
        }

        private ClipboardEventArgs ProcessClipboardEvent(ClipboardEventArgs clipboardEventArgs, ClipboardEventArgs oldEventArgs, IClipboard clipboardToSendTo)
        {
            ClipboardEventArgs sentEventArgs = null;
            if (CanProcessData && !ClipboardEventArgs.Equals(clipboardEventArgs, oldEventArgs) && !clipboardEventArgs.Data.IsNullOrWhiteSpace())
            {
                clipboardToSendTo.SendData(clipboardEventArgs.Data);
                sentEventArgs = clipboardEventArgs;
            }

            return sentEventArgs;
        }
    }
}