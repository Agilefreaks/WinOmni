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
                User32.RemoveClipboardFormatListener(_hWndSource.Handle);
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
            Justification = "Reviewed. Suppression is OK here.")]
        private IntPtr HandleClipboardMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((Msgs)msg)
            {
                case Msgs.WM_CLIPBOARDUPDATE:
                    CallDataReceived(GetClipboardText());
                    break;
            }

            return IntPtr.Zero;
        }

        private void OnHandleObtained(IntPtr windowHandle)
        {
            DisposeGetHandleObserver();
            if (windowHandle != IntPtr.Zero && !IsWatchingClippings)
            {
                _hWndSource = HwndSource.FromHwnd(windowHandle);
                if (_hWndSource != null)
                {
                    _hWndSource.AddHook(HandleClipboardMessage);
                    User32.AddClipboardFormatListener(_hWndSource.Handle);
                    IsWatchingClippings = true;
                }
            }
        }

        #endregion
    }
}