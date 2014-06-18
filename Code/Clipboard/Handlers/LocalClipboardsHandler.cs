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

        private bool skipNext = false;

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
            WindowsClipboardWrapper.DataReceived -= WindowsClipboardWrapperDataReceived;
        }

        public void PostClipping(Clipping clipping)
        {
            skipNext = true;
            WindowsClipboardWrapper.SetData(clipping.Content);
        }

        public IDisposable Subscribe(IObserver<Clipping> observer)
        {
            WindowsClipboardWrapper.DataReceived += WindowsClipboardWrapperDataReceived;
            WindowsClipboardWrapper.StartWatchingClipboard();

            return _subject.Subscribe(observer);
        }

        #endregion

        #region Methods

        private void WindowsClipboardWrapperDataReceived(object sender, ClipboardEventArgs args)
        {
            if (skipNext)
            {
                skipNext = false;
                return;
            }

            _subject.OnNext(new Clipping(args.Data));
        }

        #endregion
    }
}