namespace Clipboard.Handlers
{
    using System;
    using System.Reactive.Subjects;
    using Clipboard.Handlers.WindowsClipboard;
    using Clipboard.Models;

    public class LocalClipboardsHandler : ILocalClipboardHandler
    {
        #region Fields

        private readonly Subject<Clipping> _subject;

        private string _lastClippingContent = string.Empty;

        private bool _subscribedToWindowsClipboard;

        #endregion

        #region Constructors and Destructors

        public LocalClipboardsHandler(IWindowsClipboardWrapper windowsClipboardWrapper)
        {
            _subject = new Subject<Clipping>();
            WindowsClipboardWrapper = windowsClipboardWrapper;
        }

        #endregion

        #region Public Properties

        public IWindowsClipboardWrapper WindowsClipboardWrapper { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            WindowsClipboardWrapper.StopWatchingClipboard();
            
            if (_subscribedToWindowsClipboard)
            {
                WindowsClipboardWrapper.DataReceived -= WindowsClipboardWrapperDataReceived;
                _subscribedToWindowsClipboard = false;
            }
        }

        public void PostClipping(Clipping clipping)
        {
            _lastClippingContent = clipping.Content;
            WindowsClipboardWrapper.SetData(clipping.Content);
        }

        public IDisposable Subscribe(IObserver<Clipping> observer)
        {
            WindowsClipboardWrapper.StartWatchingClipboard();
            
            if (!_subscribedToWindowsClipboard)
            {
                WindowsClipboardWrapper.DataReceived += WindowsClipboardWrapperDataReceived;
                _subscribedToWindowsClipboard = true;
            }

            return _subject.Subscribe(observer);
        }

        #endregion

        #region Methods

        private void WindowsClipboardWrapperDataReceived(object sender, ClipboardEventArgs args)
        {
            if (_lastClippingContent.Equals(args.Data))
            {
                return;
            }


            _subject.OnNext(new Clipping(args.Data) { Source = Clipping.ClippingSourceEnum.Local} );
        }

        #endregion
    }
}