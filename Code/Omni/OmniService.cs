namespace Omni
{
    using System;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using OmniCommon;
    using OmniSync;

    public class OmniService : IOmniService
    {
        #region Fields

        private readonly IConnectionManager _connectionManager;

        #endregion

        #region Constructors and Destructors

        public OmniService(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _configurationService.AddProxyConfigurationObserver(this);
            _retryConnectionTimer.Elapsed += (sender, arguments) => Start().SubscribeOn(Scheduler.Default).Subscribe(_ => { }, _ => { });
        }

        #endregion

        #region Public Properties

        public ServiceStatusEnum Status
        {
            get
            {
                return _connectionManager.Status;
            }
        }

        public IObservable<ServiceStatusEnum> StatusChangedObservable
        {
            get
            {
                return _connectionManager.StatusChangedObservable.StartWith(_connectionManager.Status);
            }
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Unit> Start()
        {
            return _connectionManager.GoToState(ServiceStatusEnum.Started);
        }

        public void StartWithDefaultObserver()
        {
            RunObservable(Start());
        }

        public IObservable<Unit> Stop()
        {
            return _connectionManager.GoToState(ServiceStatusEnum.Stopped);
        }

        public void StopWithDefaultObserver()
        {
            RunObservable(Stop());
        }

        public void OnConfigurationChanged(ProxyConfiguration proxyConfiguration)
        {
            RestartIfStarted();
        }

        #endregion

        #region Methods

        private static void RunObservable<T>(IObservable<T> observable)
        {
            observable.SubscribeOn(Scheduler.Default).ObserveOn(Scheduler.Default).Subscribe(_ => { }, _ => { });
        }

        #endregion
    }
}