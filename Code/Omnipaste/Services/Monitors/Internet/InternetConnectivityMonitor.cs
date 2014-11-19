namespace Omnipaste.Services.Monitors.Internet
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using OmniCommon.Helpers;

    public class InternetConnectivityMonitor : IInternetConnectivityMonitor
    {
        #region Fields

        private readonly IConnectivityHelper _connectivityHelper;

        private readonly Subject<InternetConnectivityStatusEnum> _subject;

        private IDisposable _stateChangedObserver;

        #endregion

        #region Constructors and Destructors

        public InternetConnectivityMonitor(IConnectivityHelper connectivityHelper)
        {
            _connectivityHelper = connectivityHelper;
            _subject = new Subject<InternetConnectivityStatusEnum>();
        }

        #endregion

        #region Public Properties

        public IObservable<InternetConnectivityStatusEnum> ConnectivityChangedObservable
        {
            get
            {
                return _subject;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Start()
        {
            DisposeStateChangedObserver();
            _stateChangedObserver =
                _connectivityHelper.InternetConnectivityObservable.DistinctUntilChanged()
                    .Skip(1)
                    .Select(
                        isConnected =>
                        isConnected
                            ? InternetConnectivityStatusEnum.Connected
                            : InternetConnectivityStatusEnum.Disconnected)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .Subscribe(_subject);
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