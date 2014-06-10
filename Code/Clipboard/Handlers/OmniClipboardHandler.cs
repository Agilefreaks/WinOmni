namespace Clipboard.Handlers
{
    using System;
    using System.Net;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Clipboard.API;
    using Clipboard.Models;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class OmniClipboardHandler : IOmniClipboardHandler, IOmniMessageHandler
    {
        #region Fields

        private readonly Subject<Clipping> _subject;

        private IDisposable _subscription;

        #endregion

        #region Constructors and Destructors

        public OmniClipboardHandler(IClippingsApi clippingsApi, IConfigurationService configurationService)
        {
            _subject = new Subject<Clipping>();
            ClippingsApi = clippingsApi;
            ConfigurationService = configurationService;
        }

        #endregion

        #region Public Properties

        public IConfigurationService ConfigurationService { get; set; }

        public IClippingsApi ClippingsApi { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _subscription.Dispose();
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(OmniMessage value)
        {
            var clippingResponse = ClippingsApi.Last();
            clippingResponse.Wait();

            if (clippingResponse.Result.StatusCode == HttpStatusCode.OK)
            {
                _subject.OnNext(clippingResponse.Result.Data);
            }
        }

        public void PostClipping(Clipping clipping)
        {
            var postClipping = ClippingsApi.PostClipping(ConfigurationService.DeviceIdentifier, clipping.Content);
            postClipping.Wait();
        }

        public IDisposable Subscribe(IObserver<Clipping> observer)
        {
            return _subject.Subscribe(observer);
        }

        public void SubscribeTo(IObservable<OmniMessage> observable)
        {
            _subscription = observable.Where(m => m.Provider == OmniMessageTypeEnum.Clipboard).Subscribe(this);
        }

        #endregion
    }
}