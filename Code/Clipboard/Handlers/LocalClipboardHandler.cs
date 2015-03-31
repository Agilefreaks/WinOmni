namespace Clipboard.Handlers
{
    using System;
    using System.Reactive.Subjects;
    using Clipboard.Dto;
    using Clipboard.Handlers.WindowsClipboard;

    public class LocalClipboardHandler : ILocalClipboardHandler
    {
        #region Fields

        private readonly Subject<ClippingDto> _subject;

        private readonly IWindowsClipboardWrapper _windowsClipboardWrapper;

        private string _lastClippingContent = string.Empty;

        private IDisposable _windowsClipboardObserver;

        #endregion

        #region Constructors and Destructors

        public LocalClipboardHandler(IWindowsClipboardWrapper windowsClipboardWrapper)
        {
            _subject = new Subject<ClippingDto>();
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

        public void PostClipping(ClippingDto clippingDto)
        {
            _lastClippingContent = clippingDto.Content;
            _windowsClipboardWrapper.SetData(clippingDto.Content);
        }

        public IObservable<ClippingDto> Clippings
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

        private void WindowsClipboardWrapperDataReceived(ClipboardEventArgs arguments)
        {
            if (Equals(_lastClippingContent, arguments.Data))
            {
                return;
            }

            _lastClippingContent = arguments.Data;
            _subject.OnNext(new ClippingDto(arguments.Data) { Source = ClippingDto.ClippingSourceEnum.Local });
        }

        #endregion
    }
}