namespace Clipboard.Handlers.WindowsClipboard
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reactive.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using System.Windows.Interop;
    using WindowsImports;
    using OmniCommon.Interfaces;

    public class WindowsClipboardWrapper : IWindowsClipboardWrapper
    {
        #region Constants

        private const int SetDataRetryTimes = 20;

        private const int SetDataRetryIntervalInMilliseconds = 100;

        #endregion

        #region Fields

        private readonly IObservable<ClipboardEventArgs> _clippingEventsStream;

        private readonly IWindowHandleProvider _windowHandleProvider;

        private IntPtr _clipboardViewerNext;

        private IDisposable _getHandleObserver;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        private HwndSource _hWndSource;

        #endregion

        #region Constructors and Destructors

        public WindowsClipboardWrapper(IWindowHandleProvider windowHandleProvider)
        {
            _windowHandleProvider = windowHandleProvider;
            _clippingEventsStream =
                Observable.FromEventPattern<ClipboardEventArgs>(x => DataReceived += x, x => DataReceived -= x)
                    .DistinctUntilChanged(ep => ep.EventArgs.Data)
                    .Select(x => x.EventArgs);
        }

        #endregion

        #region Public Events

        public event EventHandler<ClipboardEventArgs> DataReceived;

        #endregion

        #region Public Properties

        public bool IsWatchingClippings { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            DisposeGetHandleObserver();
            if (IsWatchingClippings)
            {
                User32.ChangeClipboardChain(_hWndSource.Handle, _clipboardViewerNext);
                _clipboardViewerNext = IntPtr.Zero;
                _hWndSource.RemoveHook(HandleClipboardMessage);

                IsWatchingClippings = false;
            }
        }

        public void SetData(string data)
        {
            IDataObject dataObject = new DataObject(DataFormats.Text, data);
            var @event = new AutoResetEvent(false);
            var thread = new Thread(
                () =>
                {
                    SetClipboardData(dataObject);
                    @event.Set();
                });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            @event.WaitOne();
        }

        public void Start()
        {
            Stop();
            _getHandleObserver = _windowHandleProvider.Subscribe(OnHandleObtained, _ => { });
        }

        public void Stop()
        {
            Dispose();
        }

        public IDisposable Subscribe(IObserver<ClipboardEventArgs> observer)
        {
            return _clippingEventsStream.Subscribe(observer);
        }

        #endregion

        #region Methods

        private static IDataObject GetClipboardData()
        {
            // Data on the clipboard uses the 
            // IDataObject interface
            IDataObject dataObject;
            try
            {
                dataObject = Clipboard.GetDataObject();
            }
            catch (ExternalException)
            {
                // Copying a field definition in Access 2002 causes this sometimes?
                return null;
            }

            return dataObject;
        }

        private static string GetClipboardText()
        {
            string text = null;

            var dataObject = GetClipboardData();

            if (dataObject != null)
            {
                if (dataObject.GetDataPresent(DataFormats.UnicodeText))
                {
                    text = (string)dataObject.GetData(DataFormats.UnicodeText);
                }
            }

            return text;
        }

        private static void SetClipboardData(IDataObject dataObject)
        {
            try
            {
                Clipboard.SetDataObject(dataObject, true, SetDataRetryTimes, SetDataRetryIntervalInMilliseconds);
            }
            catch (ExternalException)
            {
            }
        }

        private void CallDataReceived(string data)
        {
            if (DataReceived != null && !string.IsNullOrWhiteSpace(data))
            {
                DataReceived(this, new ClipboardEventArgs(data));
            }
        }

        private void DisposeGetHandleObserver()
        {
            if (_getHandleObserver != null)
            {
                _getHandleObserver.Dispose();
                _getHandleObserver = null;
            }
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification ="Reviewed. Suppression is OK here. Check http://code.msdn.microsoft.com/CSWPFClipboardViewer-f601b815 for the full code")]
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

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
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

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        private string HandleDrawClipboard(int msg, IntPtr wParam, IntPtr lParam)
        {
            var data = GetClipboardText();

            // Each window that receives the WM_DRAWCLIPBOARD message 
            // must call the SendMessage function to pass the message 
            // on to the next window in the clipboard viewer chain.
            User32.SendMessage(_clipboardViewerNext, msg, wParam, lParam);

            return data;
        }

        private void OnHandleObtained(IntPtr windowHandle)
        {
            DisposeGetHandleObserver();
            if (windowHandle != IntPtr.Zero && !IsWatchingClippings)
            {
                _hWndSource = HwndSource.FromHwnd(windowHandle);

                if (_hWndSource != null)
                {
                    _clipboardViewerNext = User32.SetClipboardViewer(_hWndSource.Handle);
                    _hWndSource.AddHook(HandleClipboardMessage);

                    IsWatchingClippings = true;
                }
            }
        }

        #endregion
    }
}