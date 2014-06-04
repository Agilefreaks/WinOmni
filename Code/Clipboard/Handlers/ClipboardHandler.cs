namespace Clipboard.Handlers
{
    using System;

    public class ClipboardHandler : IClipboadHandler
    {
        #region Fields

        private IDisposable _localClipboardSubscriber;

        private IDisposable _omniClipboardSubscription;

        #endregion

        #region Constructors and Destructors

        public ClipboardHandler(
            IOmniClipboardHandler omniClipboardHandler,
            ILocalClipboardHandler localClipboardHandler)
        {
            OmniClipboardHandler = omniClipboardHandler;
            LocalClipboardHandler = localClipboardHandler;
        }

        #endregion

        #region Public Properties

        public ILocalClipboardHandler LocalClipboardHandler { get; private set; }

        public IOmniClipboardHandler OmniClipboardHandler { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void Start()
        {
            _omniClipboardSubscription = OmniClipboardHandler.Subscribe(
                // OnNext
                clipping => LocalClipboardHandler.PostClipping(clipping));

            _localClipboardSubscriber = LocalClipboardHandler.Subscribe(
                // OnNext
                clipping => OmniClipboardHandler.PostClipping(clipping));
        }

        public void Stop()
        {
            _omniClipboardSubscription.Dispose();
            OmniClipboardHandler.Dispose();

            _localClipboardSubscriber.Dispose();
            LocalClipboardHandler.Dispose();
        }

        #endregion
    }
}