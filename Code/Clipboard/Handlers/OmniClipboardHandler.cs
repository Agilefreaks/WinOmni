namespace Clipboard.Handlers
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Clipboard.API.Resources.v1;
    using Clipboard.Models;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class OmniClipboardHandler : IOmniClipboardHandler
    {
        private readonly IClippings _clippingsResource;

        #region Fields

        private readonly Subject<Clipping> _subject;

        private IDisposable _subscription;

        #endregion

        #region Constructors and Destructors

        public OmniClipboardHandler(IClippings clippingsResource, IConfigurationService configurationService)
        {
            _clippingsResource = clippingsResource;
            _subject = new Subject<Clipping>();
            ConfigurationService = configurationService;
        }

        #endregion

        #region Public Properties

        public IConfigurationService ConfigurationService { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            try
            {
                if (_subscription != null)
                {
                    _subscription.Dispose();
                }
            }
            catch (Exception exception)
            {
                ExceptionReporter.Instance.Report(exception);
            }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(OmniMessage value)
        {
            _clippingsResource.Last().RunToCompletion(
                c =>
                    {
                        c.Source = Clipping.ClippingSourceEnum.Cloud;
                        _subject.OnNext(c);
                    });
        }

        public void PostClipping(Clipping clipping)
        {
            _clippingsResource.Create(ConfigurationService.DeviceIdentifier, clipping.Content).RunToCompletion();
        }

        public IObservable<Clipping> Clippings
        {
            get
            {
                return _subject;
            }
        }

        public void Start(IObservable<OmniMessage> observable)
        {
            Stop();
            _subscription = observable.Where(m => m.Provider == OmniMessageTypeEnum.Clipboard).Subscribe(this);
        }

        public void Stop()
        {
            Dispose();
        }

        public IDisposable Subscribe(IObserver<Clipping> observer)
        {
            return _subject.Subscribe(observer);
        }

        #endregion
    }
}