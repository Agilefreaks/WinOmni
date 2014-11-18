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

        private readonly IWindowsClipboardWrapper _windowsClipboardWrapper;

        private string _lastClippingContent = string.Empty;

        private IDisposable _windowsClipboardObserver;

        #endregion

        #region Constructors and Destructors

        public LocalClipboardHandler(IWindowsClipboardWrapper windowsClipboardWrapper)
        {
            _subject = new Subject<Clipping>();
            _windowsClipboardWrapper = windowsClipboardWrapper;
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            DisposeWindowsClipboardObserver();
            _windowsClipboardWrapper.Stop();
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ClipboardEventArgs value)
        {
            WindowsClipboardWrapperDataReceived(value);
        }

        public void PostClipping(Clipping clipping)
        {
            _lastClippingContent = clipping.Content;
            _windowsClipboardWrapper.SetData(clipping.Content);
        }

        public IObservable<Clipping> Clippings
        {
            get
            {
                return _subject;
            }
        }

        public void Start()
        {
            Stop();
            _windowsClipboardWrapper.Start();
            _windowsClipboardObserver = _windowsClipboardWrapper.Subscribe(this);
        }

        public void Stop()
        {
            Dispose();
        }

        #endregion

        #region Methods

        private void DisposeWindowsClipboardObserver()
        {
            if (_windowsClipboardObserver != null)
            {
                _windowsClipboardObserver.Dispose();
                _windowsClipboardObserver = null;
            }
        }

        private void WindowsClipboardWrapperDataReceived(ClipboardEventArgs args)
        {
            if (_lastClippingContent.Equals(args.Data))
            {
                return;
            }

            _subject.OnNext(new Clipping(args.Data) { Source = Clipping.ClippingSourceEnum.Local });
        }

        #endregion
    }
}