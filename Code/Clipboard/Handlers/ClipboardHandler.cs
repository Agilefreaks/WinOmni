namespace Clipboard.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Clipboard.Models;
    using OmniCommon.Models;

    public class ClipboardHandler : IClipboardHandler
    {
        #region Fields

        private readonly IObservable<Clipping> _clippingsObservable;

        private readonly List<IDisposable> _observers = new List<IDisposable>();

        #endregion

        #region Constructors and Destructors

        public ClipboardHandler(
            IOmniClipboardHandler omniClipboardHandler,
            ILocalClipboardHandler localClipboardHandler)
        {
            OmniClipboardHandler = omniClipboardHandler;
            LocalClipboardHandler = localClipboardHandler;

            _clippingsObservable = OmniClipboardHandler.Merge(LocalClipboardHandler);
        }

        #endregion

        #region Public Properties

        public ILocalClipboardHandler LocalClipboardHandler { get; private set; }

        public IOmniClipboardHandler OmniClipboardHandler { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void Start(IObservable<OmniMessage> omniMessageObservable)
        {
            Stop();
            OmniClipboardHandler.Start(omniMessageObservable);
            LocalClipboardHandler.Start();

            _observers.Add(
                OmniClipboardHandler
                .SubscribeOn(Scheduler.Default)
                .Subscribe(
                    // OnNext
                    clipping => LocalClipboardHandler.PostClipping(clipping),
                    _ => { }));

            _observers.Add(
                LocalClipboardHandler
                .SubscribeOn(Scheduler.Default)
                .Subscribe(
                    // OnNext
                    clipping => OmniClipboardHandler.PostClipping(clipping),
                    _ => { }));
        }

        public void Stop()
        {
            OmniClipboardHandler.Stop();
            LocalClipboardHandler.Stop();
            _observers.ForEach(observer => observer.Dispose());
        }

        public IDisposable Subscribe(IObserver<Clipping> observer)
        {
            return _clippingsObservable.Subscribe(observer);
        }

        #endregion
    }
}