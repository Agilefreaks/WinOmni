namespace Omnipaste.Services
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using Omni;
    using OmniCommon.Helpers;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Services.Monitors.Internet;

    public class ConnectionEventSupervisor : IConnectionEventSupervisor
    {
        private readonly IOmniService _omniService;

        private IDisposable _internetConnectivityObserver;

        public ConnectionEventSupervisor(IOmniService omniService)
        {
            _omniService = omniService;
        }

        [Inject]
        public IInternetConnectivityMonitor InternetConnectivityMonitor { get; set; }

        public void Start()
        {
            _internetConnectivityObserver =
                InternetConnectivityMonitor.ConnectivityChangedObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(OnInternetConnectivityChanged);
        }

        public void Stop()
        {
            if(_internetConnectivityObserver != null) _internetConnectivityObserver.Dispose();
        }

        private void OnInternetConnectivityChanged(bool haveInternetConnectivity)
        {
            if (haveInternetConnectivity)
            {
                _omniService.Start();
            }
            else
            {
                _omniService.Stop();
            }
        }
    }
}