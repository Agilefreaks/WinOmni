namespace Clipboard.Handlers
{
    using System;
    using System.Reactive.Subjects;
    using Clipboard.Handlers.WindowsClipboard;
    using Clipboard.Models;

    public class LocalClipboardHandler : ILocalClipboardHandler
    {
        #region Fields

        private readonly Subject<Clipping> _subject;

        private string _lastClippingContent = string.Empty;

        private IDisposable _windowsClipboardSubscription;

        #endregion

        #region Constructors and Destructors

        public LocalClipboardHandler(IWindowsClipboardWrapper windowsClipboardWrapper)
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
            if (_windowsClipboardSubscription != null)
            {
                _windowsClipboardSubscription.Dispose();
                _windowsClipboardSubscription = null;
            }

            WindowsClipboardWrapper.Dispose();
        }

        public void PostClipping(Clipping clipping)
        {
            _lastClippingContent = clipping.Content;
            WindowsClipboardWrapper.SetData(clipping.Content);
        }

        public IDisposable Subscribe(IObserver<Clipping> observer)
        {
            if (_windowsClipboardSubscription == null)
            {
                _windowsClipboardSubscription = WindowsClipboardWrapper.Subscribe(this);
            }

            return _subject.Subscribe(observer);
        }

        #endregion

        #region Methods

        private void WindowsClipboardWrapperDataReceived(ClipboardEventArgs args)
        {
            if (_lastClippingContent.Equals(args.Data))
            {
                return;
            }

            _subject.OnNext(new Clipping(args.Data) { Source = Clipping.ClippingSourceEnum.Local} );
        }

        #endregion

        public void OnNext(ClipboardEventArgs value)
        {
            WindowsClipboardWrapperDataReceived(value);
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
    }
}