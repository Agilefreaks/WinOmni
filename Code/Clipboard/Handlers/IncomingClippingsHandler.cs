namespace Clipboard.Handlers
{
    using System;
    using System.Net;
    using System.Reactive.Linq;
    using WindowsClipboard;
    using Caliburn.Micro;
    using Clipboard.API;
    using Ninject;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class IncomingClippingsHandler : IOmniMessageHandler
    {
        private readonly IEventAggregator _eventAggregator;

        private IDisposable _subscription;

        [Inject]
        public IClippingsApi ClippingsApi { get; set; }

        public IEventAggregator EventAggregator
        {
            get
            {
                return _eventAggregator;
            }
        }

        public IncomingClippingsHandler(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void OnNext(OmniMessage value)
        {
            var clippingResponse = ClippingsApi.Last();
            clippingResponse.Wait();

            if (clippingResponse.Result.StatusCode == HttpStatusCode.OK)
            {
                EventAggregator.PublishOnCurrentThread(new ClipboardData(null, clippingResponse.Result.Data.Content));
            }
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        public void SubscribeTo(IObservable<OmniMessage> observable)
        {
            _subscription = observable.Where(m => m.Provider == OmniMessageTypeEnum.Clipboard).Subscribe(this);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}