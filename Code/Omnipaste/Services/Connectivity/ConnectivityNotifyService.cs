namespace Omnipaste.Services.Connectivity
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public class ConnectivityNotifyService : IConnectivityNotifyService
    {
        #region Fields

        private readonly IConnectivityHelper _connectivityHelper;

        private readonly TimeSpan _defaultCheckInterval = TimeSpan.FromSeconds(5);

        private readonly IObservable<bool> _stateChangedObservable;

        private readonly ReplaySubject<bool> _subject;

        private IDisposable _stateChangedObserver;

        #endregion

        #region Constructors and Destructors

        public ConnectivityNotifyService(IConnectivityHelper connectivityHelper, TimeSpan? checkInterval = null)
        {
            _connectivityHelper = connectivityHelper;
            _subject = new ReplaySubject<bool>(0);
            _stateChangedObservable =
                Observable.Timer(TimeSpan.Zero, checkInterval ?? _defaultCheckInterval)
                    .Select(_ => CurrentlyConnected)
                    .DistinctUntilChanged()
                    .Skip(1);
        }

        #endregion

        #region Public Properties

        public IObservable<bool> ConnectivityChangedObservable
        {
            get
            {
                return _subject;
            }
        }

        public bool CurrentlyConnected
        {
            get
            {
                return _connectivityHelper.InternetConnected;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Start()
        {
            DisposeStateChangedObserver();
            _stateChangedObserver =
                _stateChangedObservable.SubscribeOn(Scheduler.Default).ObserveOn(Scheduler.Default).Subscribe(_subject);
        }

        public void Stop()
        {
            DisposeStateChangedObserver();
        }

        #endregion

        #region Methods

        private void DisposeStateChangedObserver()
        {
            if (_stateChangedObserver != null)
            {
                _stateChangedObserver.Dispose();
            }
        }

        #endregion
    }
}