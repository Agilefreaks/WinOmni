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
    using OmniCommon.Helpers;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Services.Monitors.Internet;
    using Omnipaste.Services.Monitors.Power;
    using Omnipaste.Services.Monitors.User;
    using OmniSync;

    public class ConnectivitySupervisor : IConnectivitySupervisor
    {
        #region Fields

        private readonly IList<IDisposable> _eventObservers;

        private readonly IOmniService _omniService;

        private readonly TimeSpan _reconnectInterval = TimeSpan.FromSeconds(5);

        private IDisposable _reconnectObserver;

        private readonly IObservable<Unit> _reconnectObservable;

        #endregion

        #region Constructors and Destructors

        public ConnectivitySupervisor(IOmniService omniService)
        {
            _eventObservers = new List<IDisposable>();
            _omniService = omniService;
            _reconnectObservable =
                Observable.Timer(_reconnectInterval, SchedulerProvider.Default)
                    .Select(_ => _omniService.Stop())
                    .Switch()
                    .Retry()
                    .Select(_ => _omniService.Start())
                    .Switch()
                    .Retry()
                    .Select(_ => Observable.Start(StopReconnectProcess))
                    .Switch();
            _omniService.StatusChangedObservable.Where(status => status == OmniServiceStatusEnum.Started)
                .SubscribeOn(SchedulerProvider.Default)
                .ObserveOn(SchedulerProvider.Default)
                .SubscribeAndHandleErrors(_ => StopReconnectProcess());
        }

        #endregion

        #region Public Properties

        [Inject]
        public IInternetConnectivityMonitor InternetConnectivityMonitor { get; set; }

        [Inject]
        public IPowerMonitor PowerMonitor { get; set; }

        [Inject]
        public IUserMonitor UserMonitor { get; set; }

        [Inject]
        public IWebSocketMonitor WebSocketMonitor { get; set; }

        #endregion

        #region Public Methods and Operators

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

            _eventObservers.Add(
                WebSocketMonitor.ConnectionObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(WebSocketConnectionChanged));
        }

        public void Stop()
        {
            _eventObservers.ForEach(observer => observer.Dispose());
        }

        #endregion

        #region Methods

        private void OnInternetConnectivityChanged(InternetConnectivityStatusEnum newState)
        {
            switch (newState)
            {
                case InternetConnectivityStatusEnum.Connected:
                    OnStateChangeRequired(OmniServiceStatusEnum.Started);
                    break;
                case InternetConnectivityStatusEnum.Disconnected:
                    OnStateChangeRequired(OmniServiceStatusEnum.Stopped);
                    break;
            }
        }

        private void OnPowerModeChanged(PowerModes newMode)
        {
            switch (newMode)
            {
                case PowerModes.Resume:
                    if (InternetConnectivityMonitor.CurrentlyConnected)
                    {
                        OnStateChangeRequired(OmniServiceStatusEnum.Started);
                    }
                    break;
                case PowerModes.Suspend:
                    OnStateChangeRequired(OmniServiceStatusEnum.Stopped);
                    break;
            }
        }

        private void OnStateChangeRequired(OmniServiceStatusEnum newState)
        {
            var observable = newState == OmniServiceStatusEnum.Started ? _omniService.Start() : _omniService.Stop();
            observable.SubscribeAndHandleErrors();
        }

        private void OnUserEventReceived(UserEventTypeEnum eventType)
        {
            switch (eventType)
            {
                case UserEventTypeEnum.Connect:
                    OnStateChangeRequired(OmniServiceStatusEnum.Started);
                    break;
                case UserEventTypeEnum.Disconnect:
                    OnStateChangeRequired(OmniServiceStatusEnum.Stopped);
                    break;
            }
        }

        private void Reconnect()
        {
            StopReconnectProcess();
            _reconnectObserver =
                _reconnectObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors();
        }

        private void StopReconnectProcess()
        {
            if (_reconnectObserver != null)
            {
                _reconnectObserver.Dispose();
            }
        }

        private void WebSocketConnectionChanged(WebSocketConnectionStatusEnum newState)
        {
            switch (newState)
            {
                case WebSocketConnectionStatusEnum.Disconnected:
                    if (_omniService.State == OmniServiceStatusEnum.Started && !_omniService.InTransition)
                    {
                        Reconnect();
                    }
                    break;
            }
        }

        #endregion
    }
}