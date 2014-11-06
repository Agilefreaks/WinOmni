namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Castle.Core.Internal;
    using Microsoft.Win32;
    using Ninject;
    using Omni;
    using OmniCommon.Helpers;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Services.Monitors.Internet;
    using Omnipaste.Services.Monitors.Power;
    using Omnipaste.Services.Monitors.User;

    public class ConnectionEventSupervisor : IConnectionEventSupervisor
    {
        private readonly IOmniService _omniService;

        private readonly IList<IDisposable> _eventObservers;

        public ConnectionEventSupervisor(IOmniService omniService)
        {
            _eventObservers = new List<IDisposable>();
            _omniService = omniService;
        }

        [Inject]
        public IInternetConnectivityMonitor InternetConnectivityMonitor { get; set; }

        [Inject]
        public IPowerMonitor PowerMonitor { get; set; }

        [Inject]
        public IUserMonitor UserMonitor { get; set; }

        public void Start()
        {
            _eventObservers.Add(
                InternetConnectivityMonitor.ConnectivityChangedObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(OnInternetConnectivityChanged));

            _eventObservers.Add(
                PowerMonitor.PowerModesObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(OnPowerModeChanged));

            _eventObservers.Add(
                UserMonitor.UserEventObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(OnUserEventReceived));
        }

        public void Stop()
        {
            _eventObservers.ForEach(observer => observer.Dispose());
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

        private void OnPowerModeChanged(PowerModes newMode)
        {
            switch (newMode)
            {
                case PowerModes.Resume:
                    if (InternetConnectivityMonitor.CurrentlyConnected)
                    {
                        _omniService.Start();
                    }
                    break;
                case PowerModes.Suspend:
                    _omniService.Stop();
                    break;
            }
        }

        private void OnUserEventReceived(UserEventTypeEnum eventType)
        {
            switch (eventType)
            {
                case UserEventTypeEnum.Connect:
                    _omniService.Start();
                    break;
                case UserEventTypeEnum.Disconnect:
                    _omniService.Stop();
                    break;
            }
        }
    }
}