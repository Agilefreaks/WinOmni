namespace Clipboard.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Clipboard.Models;
    using OmniCommon.Helpers;
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

            _clippingsObservable = OmniClipboardHandler.Clippings.Merge(LocalClipboardHandler.Clippings);
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
                OmniClipboardHandler.Clippings
                .SubscribeOn(SchedulerProvider.Default)
                .Subscribe(
                    // OnNext
                    clipping => LocalClipboardHandler.PostClipping(clipping),
                    _ => { }));

            _observers.Add(
                LocalClipboardHandler.Clippings
                .SubscribeOn(SchedulerProvider.Default)
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
            _observers.Clear();
        }

        public IDisposable Subscribe(IObserver<Clipping> observer)
        {
            return _clippingsObservable.Subscribe(observer);
        }

        #endregion
    }
}