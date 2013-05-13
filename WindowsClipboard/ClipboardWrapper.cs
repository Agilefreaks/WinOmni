using System;
using System.Windows.Forms;
using Ninject;
using WindowsClipboard.Imports;

namespace WindowsClipboard
{
    public class ClipboardWrapper : IClipboardWrapper
    {
        IntPtr _clipboardViewerNext;
        private IntPtr _handle;

        [Inject]
        public IClipboardAdapter ClipboardAdapter { get; set; }

        public ClipboardMessageHandleResult HandleClipboardMessage(Message message)
        {
            var messageHandleResult = new ClipboardMessageHandleResult { MessageHandled = true };
            switch ((Msgs)message.Msg)
            {
                //
                // The WM_DRAWCLIPBOARD message is sent to the first window 
                // in the clipboard viewer chain when the content of the 
                // clipboard changes. This enables a clipboard viewer 
                // window to display the new content of the clipboard. 
                //
                case Msgs.WM_DRAWCLIPBOARD:
                    messageHandleResult.MessageData = HandleDrawClipboard(message);
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
                    messageHandleResult.MessageHandled = false;
                    break;
            }

            return messageHandleResult;
        }

        public string GetClipboardText()
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

        public void Initialize(IntPtr handle)
        {
            _handle = handle;
            _clipboardViewerNext = User32.SetClipboardViewer(handle);
        }

        public void Dispose()
        {
            User32.ChangeClipboardChain(_handle, _clipboardViewerNext);
        }

        public void SendToClipboard(string data)
        {
            ClipboardAdapter.SetData(data);
        }

        private IDataObject GetClipboardData()
        {
            //
            // Data on the clipboard uses the 
            // IDataObject interface
            //
            IDataObject iData;
            try
            {
                iData = ClipboardAdapter.GetDataObject();
            }
            catch (System.Runtime.InteropServices.ExternalException externEx)
            {
                // Copying a field definition in Access 2002 causes this sometimes?
                return null;
            }

            return iData;
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
    }
}