using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Common.Logging;
using Ninject;
using OmniCommon;
using OmniCommon.ExtensionMethods;
using WindowsClipboard.Imports;
using WindowsClipboard.Interfaces;

namespace WindowsClipboard
{
    public class WindowsClipboardWrapper : IWindowsClipboardWrapper
    {
        public event EventHandler<ClipboardEventArgs> DataReceived;

        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

        private IntPtr _clipboardViewerNext;

        [Inject]
        public IDelegateClipboardMessageHandling ClipboardMessageDelegator { get; set; }

        public void StartWatchingClipboard()
        {
            ClipboardMessageDelegator.HandleClipboardMessage += HandleClipboardMessage;
            _clipboardViewerNext = User32.SetClipboardViewer(ClipboardMessageDelegator.Handle);
        }

        public void StopWatchingClipboard()
        {
            User32.ChangeClipboardChain(ClipboardMessageDelegator.Handle, _clipboardViewerNext);
            ClipboardMessageDelegator.HandleClipboardMessage -= HandleClipboardMessage;
        }

        public void SetData(string data)
        {
            RunOnAnSTAThread(() => Clipboard.SetData(DataFormats.Text, data));
        }

        private bool HandleClipboardMessage(ref Message message)
        {
            var messageHandled = true;
            switch ((Msgs)message.Msg)
            {
                //
                // The WM_DRAWCLIPBOARD message is sent to the first window 
                // in the clipboard viewer chain when the content of the 
                // clipboard changes. This enables a clipboard viewer 
                // window to display the new content of the clipboard. 
                //
                case Msgs.WM_DRAWCLIPBOARD:
                    CallDataReceived(HandleDrawClipboard(message));
                    break;

                //
                // The WM_CHANGECBCHAIN message is sent to the first window 
                // in the clipboard viewer chain when a window is being 
                // removed from the chain. 
                //
                case Msgs.WM_CHANGECBCHAIN:
                    HandleClipboardChainChanged(message);
                    break;

                //
                // Let the form process the messages that we are
                // not interested in
                //
                default:
                    messageHandled = false;
                    break;
            }

            return messageHandled;
        }

        private static string GetClipboardText()
        {
            string text = null;
            var iData = GetClipboardData();
            if (iData != null)
            {
                if (iData.GetDataPresent(DataFormats.Text))
                {
                    text = (string)iData.GetData(DataFormats.Text);
                }
                else
                {
                    text = "(cannot display this format)";
                }
            }

            return text;
        }

        private static IDataObject GetClipboardData()
        {
            //
            // Data on the clipboard uses the 
            // IDataObject interface
            //
            IDataObject iData;
            try
            {
                iData = Clipboard.GetDataObject();
            }
            catch (ExternalException externalException)
            {
                // Copying a field definition in Access 2002 causes this sometimes?
                Logger.Error(externalException);
                return null;
            }

            return iData;
        }

        private string HandleDrawClipboard(Message message)
        {
            var data = GetClipboardText();

            //
            // Each window that receives the WM_DRAWCLIPBOARD message 
            // must call the SendMessage function to pass the message 
            // on to the next window in the clipboard viewer chain.
            //
            User32.SendMessage(_clipboardViewerNext, message.Msg, message.WParam, message.LParam);

            return data;
        }

        private void HandleClipboardChainChanged(Message message)
        {
            // When a clipboard viewer window receives the WM_CHANGECBCHAIN message, 
            // it should call the SendMessage function to pass the message to the 
            // next window in the chain, unless the next window is the window 
            // being removed. In this case, the clipboard viewer should save 
            // the handle specified by the lParam parameter as the next window in the chain. 

            //
            // wParam is the Handle to the window being removed from 
            // the clipboard viewer chain 
            // lParam is the Handle to the next window in the chain 
            // following the window being removed. 
            if (message.WParam == _clipboardViewerNext)
            {
                //
                // If wParam is the next clipboard viewer then it
                // is being removed so update pointer to the next
                // window in the clipboard chain
                //
                _clipboardViewerNext = message.LParam;
            }
            else
            {
                User32.SendMessage(_clipboardViewerNext, message.Msg, message.WParam, message.LParam);
            }
        }

        private void CallDataReceived(string data)
        {
            if (DataReceived != null && !data.IsNullOrWhiteSpace())
            {
                DataReceived(this, new ClipboardEventArgs(data));
            }
        }

        private static void RunOnAnSTAThread(Action action)
        {
            var @event = new AutoResetEvent(false);
            var thread = new Thread(() =>
                {
                    action();
                    @event.Set();
                });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            @event.WaitOne();
        }
    }
}