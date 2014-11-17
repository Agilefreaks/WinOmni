namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Linq;
    using Castle.Core.Internal;
    using Microsoft.Win32;
    using Ninject;
    using Omni;
    using OmniCommon;
    using OmniCommon.Helpers;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Services.Monitors.Internet;
    using Omnipaste.Services.Monitors.Power;
    using Omnipaste.Services.Monitors.ProxyConfiguration;
    using Omnipaste.Services.Monitors.User;
    using OmniSync;

    public class ConnectivitySupervisor : IConnectivitySupervisor
    {
        #region Static Fields

        private static readonly TimeSpan DefaultReconnectInterval = TimeSpan.FromSeconds(5);

        #endregion

        #region Fields

        private readonly IList<IDisposable> _eventObservers;

        private readonly IOmniService _omniService;

        private IDisposable _connectObserver;

        #endregion

        #region Constructors and Destructors

        public ConnectivitySupervisor(IOmniService omniService)
        {
            _eventObservers = new List<IDisposable>();
            _omniService = omniService;
            _omniService.StatusChangedObservable.Where(status => status == OmniServiceStatusEnum.Started)
                .SubscribeOn(SchedulerProvider.Default)
                .ObserveOn(SchedulerProvider.Default)
                .SubscribeAndHandleErrors(_ => StopConnectProcess());
        }

        #endregion

        #region Public Properties

        [Inject]
        public IInternetConnectivityMonitor InternetConnectivityMonitor { get; set; }

        [Inject]
        public IPowerMonitor PowerMonitor { get; set; }

        [Inject]
        public IProxyConfigurationMonitor ProxyConfigurationMonitor { get; set; }

        [Inject]
        public IUserMonitor UserMonitor { get; set; }

        [Inject]
        public IWebSocketMonitor WebSocketMonitor { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Start()
        {
            Stop();
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

            _eventObservers.Add(
                WebSocketMonitor.ConnectionObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(WebSocketConnectionChanged));

            _eventObservers.Add(
                ProxyConfigurationMonitor.ProxyConfigurationObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(OnProxyConfigurationChanged));
        }

        public void Stop()
        {
            _eventObservers.ForEach(observer => observer.Dispose());
        }

        #endregion

        #region Methods

        private void OnConnectionLost()
        {
            if (_omniService.State != OmniServiceStatusEnum.Started || _omniService.InTransition)
            {
                return;
            }

            StartOmniService();
        }

        private void OnInternetConnectivityChanged(InternetConnectivityStatusEnum newState)
        {
            switch (newState)
            {
                case InternetConnectivityStatusEnum.Disconnected:
                    OnConnectionLost();
                    break;
            }
        }

        private void OnPowerModeChanged(PowerModes newMode)
        {
            switch (newMode)
            {
                case PowerModes.Resume:
                    StartOmniService();
                    break;
                case PowerModes.Suspend:
                    StopOmniService();
                    break;
            }
        }

        private void OnProxyConfigurationChanged(ProxyConfiguration newProxyConfiguration)
        {
            OnConnectionLost();
        }

        private void OnUserEventReceived(UserEventTypeEnum eventType)
        {
            switch (eventType)
            {
                case UserEventTypeEnum.Connect:
                    StartOmniService();
                    break;
                case UserEventTypeEnum.Disconnect:
                    StopOmniService();
                    break;
            }
        }

        private void StartOmniService()
        {
            StopConnectProcess();
            _connectObserver = GetConnectObservable().SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors();
        }

        private void StopConnectProcess()
        {
            if (_connectObserver != null)
            {
                _connectObserver.Dispose();
            }
        }

        private void StopOmniService()
        {
            _omniService.Stop().SubscribeAndHandleErrors();
        }

        private void WebSocketConnectionChanged(WebSocketConnectionStatusEnum newState)
        {
            switch (newState)
            {
                case WebSocketConnectionStatusEnum.Disconnected:
                    OnConnectionLost();
                    break;
            }
        }

        private IObservable<Unit> GetConnectObservable()
        {
            return Observable.Defer(() => _omniService.Stop())
                .Select(_ => _omniService.Start())
                .Switch()
                .Select(_ => Observable.Start(StopConnectProcess))
                .Switch()
                .RetryAfter(DefaultReconnectInterval);
        }

        #endregion
    }
}