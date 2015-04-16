namespace Omnipaste.Framework.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Castle.Core.Internal;
    using Microsoft.Win32;
    using Ninject;
    using Omni;
    using OmniCommon;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Models;
    using OmniCommon.Settings;
    using Omnipaste.Framework.Services.Monitors.Credentials;
    using Omnipaste.Framework.Services.Monitors.Internet;
    using Omnipaste.Framework.Services.Monitors.Power;
    using Omnipaste.Framework.Services.Monitors.ProxyConfiguration;
    using Omnipaste.Framework.Services.Monitors.User;
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

        [Inject]
        public ICredentialsMonitor CredentialsMonitor { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Start()
        {
            SimpleLogger.Log("Starting connectivity supervisor");
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
                ProxyConfigurationMonitor.SettingObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(OnProxyConfigurationChanged));
            
            _eventObservers.Add(
                CredentialsMonitor.SettingObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(OnCredentialsChanged));
            SimpleLogger.Log("Started connectivity supervisor");
        }

        public void Stop()
        {
            SimpleLogger.Log("Stopping connectivity supervisor");
            _eventObservers.ForEach(observer => observer.Dispose());
            _eventObservers.Clear();
            SimpleLogger.Log("Stopped connectivity supervisor");
        }

        #endregion

        #region Methods

        private void OnConnectionLost()
        {
            SimpleLogger.Log("Monitor requested reconnect");
            if (_omniService.State != OmniServiceStatusEnum.Started || _omniService.InTransition)
            {
                SimpleLogger.Log("OmniService not started or in transition, reconnect canceled");
                return;
            }

            RestartOmniService();
        }

        private void OnCredentialsChanged(OmnipasteCredentials credentials)
        {
            SimpleLogger.Log("User credentials changed");
            StopOmniService();
        }

        private void OnInternetConnectivityChanged(InternetConnectivityStatusEnum newState)
        {
            switch (newState)
            {
                case InternetConnectivityStatusEnum.Disconnected:
                    SimpleLogger.Log("Lost internet connection");
                    OnConnectionLost();
                    break;
            }
        }

        private void OnPowerModeChanged(PowerModes newMode)
        {
            switch (newMode)
            {
                case PowerModes.Resume:
                    SimpleLogger.Log("Resuming from power mode event.");
                    RestartOmniService();
                    break;
                case PowerModes.Suspend:
                    SimpleLogger.Log("Stopping from power mode event");
                    StopOmniService();
                    break;
            }
        }

        private void OnProxyConfigurationChanged(ProxyConfiguration newProxyConfiguration)
        {
            SimpleLogger.Log("Proxy configuration changed");
            OnConnectionLost();
        }

        private void OnUserEventReceived(UserEventTypeEnum eventType)
        {
            switch (eventType)
            {
                case UserEventTypeEnum.Connect:
                    SimpleLogger.Log("User requested connect");
                    RestartOmniService();
                    break;
                case UserEventTypeEnum.Disconnect:
                    SimpleLogger.Log("User requested disconnect");
                    StopOmniService();
                    break;
            }
        }

        private void RestartOmniService()
        {
            SimpleLogger.Log("Restarting OmniService");
            if (_connectObserver != null)
            {
                SimpleLogger.Log("A reconnect process was already started");
                return;
            }

            _connectObserver =
                Observable.Defer(
                    () => _omniService.Stop())
                    .Select(_ => _omniService.Start())
                    .Switch()
                    .Do(
                        _ =>
                            {
                                SimpleLogger.Log("Successfully restarted OmniService, stoping reconnect process.");
                                _connectObserver.Dispose();
                                _connectObserver = null;
                            })
                    .RetryUntil(
                        _ =>
                            {
                                SimpleLogger.Log("Verifying if should retry to reconnect.");
                                return _omniService.State != OmniServiceStatusEnum.Started;
                            }, DefaultReconnectInterval)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors();
        }
        
        private void StopOmniService()
        {
            _omniService.Stop().RunToCompletion();
        }

        private void WebSocketConnectionChanged(WebSocketConnectionStatusEnum newState)
        {
            switch (newState)
            {
                case WebSocketConnectionStatusEnum.Disconnected:
                    SimpleLogger.Log("Websocket connection lost");
                    OnConnectionLost();
                    break;
            }
        }

        #endregion
    }
}