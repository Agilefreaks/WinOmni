namespace WindowsClipboard
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using System.Windows.Interop;
    using Common.Logging;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using global::WindowsClipboard.Interfaces;
    using WindowsImports;

    public class WindowsClipboardWrapper : IWindowsClipboardWrapper
    {
        public event EventHandler<ClipboardEventArgs> DataReceived;

        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private HwndSource _hWndSource;

        private IntPtr _clipboardViewerNext;

        [Inject]
        public IDelegateClipboardMessageHandling ClipboardMessageDelegator { get; set; }

        public void StartWatchingClipboard()
        {
            var handle = ClipboardMessageDelegator.GetHandle();

            _hWndSource = HwndSource.FromHwnd(handle);
            _hWndSource.AddHook(HandleClipboardMessage); 

            _clipboardViewerNext = User32.SetClipboardViewer(_hWndSource.Handle);
        }

        public void StopWatchingClipboard()
        {
            User32.ChangeClipboardChain(_hWndSource.Handle, _clipboardViewerNext);
            _clipboardViewerNext = IntPtr.Zero;
            _hWndSource.RemoveHook(HandleClipboardMessage);
        }

        public void SetData(string data)
        {
            RunOnAnSTAThread(() => Clipboard.SetData(DataFormats.Text, data));
        }

        private static string GetClipboardText()
        {
            string text = null;
            var dataObject = GetClipboardData();
            if (dataObject != null)
            {
                if (dataObject.GetDataPresent(DataFormats.Text))
                {
                    text = (string)dataObject.GetData(DataFormats.Text);
                }
            }

            return text;
        }

        private static IDataObject GetClipboardData()
        {
            // Data on the clipboard uses the 
            // IDataObject interface
            IDataObject dataObject;
            try
            {
                dataObject = Clipboard.GetDataObject();
            }
            catch (ExternalException externalException)
            {
                // Copying a field definition in Access 2002 causes this sometimes?
                Logger.Error(externalException);
                return null;
            }

            return dataObject;
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

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private string HandleDrawClipboard(int msg, IntPtr wParam, IntPtr lParam)
        {
            var data = GetClipboardText();

            // Each window that receives the WM_DRAWCLIPBOARD message 
            // must call the SendMessage function to pass the message 
            // on to the next window in the clipboard viewer chain.
            User32.SendMessage(_clipboardViewerNext, msg, wParam, lParam);

            return data;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private IntPtr HandleClipboardMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((Msgs)msg)
            {
                // The WM_DRAWCLIPBOARD message is sent to the first window 
                // in the clipboard viewer chain when the content of the 
                // clipboard changes. This enables a clipboard viewer 
                // window to display the new content of the clipboard. 
                case Msgs.WM_DRAWCLIPBOARD:
                    CallDataReceived(HandleDrawClipboard(msg, wParam, lParam));
                    break;

                // The WM_CHANGECBCHAIN message is sent to the first window 
                // in the clipboard viewer chain when a window is being 
                // removed from the chain. 
                case Msgs.WM_CHANGECBCHAIN:
                    HandleClipboardChainChanged(msg, wParam, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here. Check http://code.msdn.microsoft.com/CSWPFClipboardViewer-f601b815 for the full code")]
        private void HandleClipboardChainChanged(int msg, IntPtr wParam, IntPtr lParam)
        {
            // When a clipboard viewer window receives the WM_CHANGECBCHAIN message, 
            // it should call the SendMessage function to pass the message to the 
            // next window in the chain, unless the next window is the window 
            // being removed. In this case, the clipboard viewer should save 
            // the handle specified by the lParam parameter as the next window in the chain. 

            // wParam is the Handle to the window being removed from 
            // the clipboard viewer chain 
            // lParam is the Handle to the next window in the chain 
            // following the window being removed. 
            if (wParam == _clipboardViewerNext)
            {
                // If wParam is the next clipboard viewer then it
                // is being removed so update pointer to the next
                // window in the clipboard chain
                _clipboardViewerNext = lParam;
            }
            else
            {
                User32.SendMessage(_clipboardViewerNext, msg, wParam, lParam);
            }
        }

        private void CallDataReceived(string data)
        {
            if (DataReceived != null && !data.IsNullOrWhiteSpace())
            {
                DataReceived(this, new ClipboardEventArgs(data));
            }
        }
    }
}