using Caliburn.Micro;

namespace Clipboard.Handlers
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    class ClippingHandler : IOmniMessageHandler
    {
        private readonly IEventAggregator _eventAggregator;

        private IDisposable _subscription;

        [Inject]
        public IClippingsAPI ClippingsAPI { get; set; }

        public ClippingHandler(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void OnNext(OmniMessage value)
        {
            var lastClipping = ClippingsAPI.LastClipping();
            _eventAggregator.Publish(lastClipping.Data);
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        public void SubscribeTo(IObservable<OmniMessage> observable)
        {
            _subscription = observable.Where(m => m.Type == OmniMessageTypeEnum.Clipping).Subscribe(this);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}