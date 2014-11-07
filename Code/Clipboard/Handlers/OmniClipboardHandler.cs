namespace Clipboard.Handlers
{
    using System;
    using System.Diagnostics;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Clipboard.API.Resources.v1;
    using Clipboard.Models;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class OmniClipboardHandler : IOmniClipboardHandler
    {
        #region Fields

        private readonly Subject<Clipping> _subject;

        private IDisposable _subscription;

        #endregion

        #region Constructors and Destructors

        public OmniClipboardHandler(IClippings clippings, IConfigurationService configurationService)
        {
            _subject = new Subject<Clipping>();
            Clippings = clippings;
            ConfigurationService = configurationService;
        }

        #endregion

        #region Public Properties

        public IClippings Clippings { get; private set; }

        public IConfigurationService ConfigurationService { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            if (_subscription != null)
            {
                try
                {
                    _subscription.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
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
            Clippings.Last().Subscribe(
                // OnNext
                c =>
                    {
                        c.Source = Clipping.ClippingSourceEnum.Cloud;
                        _subject.OnNext(c);
                    },
                // OnError
                e => Debugger.Break());
        }

        public void PostClipping(Clipping clipping)
        {
            Clippings.Create(ConfigurationService.DeviceIdentifier, clipping.Content).Subscribe(_ => { }, _ => { });
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