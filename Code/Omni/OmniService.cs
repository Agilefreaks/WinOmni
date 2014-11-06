namespace Omni
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using OmniCommon;
    using System.Threading;
    using BugFreak;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniSync;

    public class OmniService : IOmniService
    {
        #region Constants

        private const int NoSwitchInProgress = 0;

        #endregion

        #region Fields

        protected IWebsocketConnection WebsocketConnection;

        private readonly ReplaySubject<OmniServiceStatusEnum> _statusChangedSubject;

        private OmniServiceStatusEnum _state;

        private int _migrationState;

        #endregion

        #region Constructors and Destructors

        public OmniService()
        {
            _statusChangedSubject = new ReplaySubject<OmniServiceStatusEnum>(1);
            State = OmniServiceStatusEnum.Stopped;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IDevices Devices { get; set; }

        public OmniServiceStatusEnum State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                _statusChangedSubject.OnNext(value);
            }
        }

        [Inject]
        public IKernel Kernel { get; set; }

        public IObservable<OmniServiceStatusEnum> StatusChangedObservable
        {
            get
            {
                return _statusChangedSubject;
            }
        }

        [Inject]
        public IWebSocketMonitor WebSocketMonitor { get; set; }

        [Inject]
        public IWebsocketConnectionFactory WebsocketConnectionFactory { get; set; }

        public bool InTransition
        {
            get
            {
                return _migrationState != NoSwitchInProgress;
            }
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Unit> Start()
        {
            return SwitchToState(OmniServiceStatusEnum.Started);
        }

        public IObservable<Unit> Stop()
        {
            return SwitchToState(OmniServiceStatusEnum.Stopped);
        }

        #endregion

        #region Methods

        private IObservable<Device> ActivateDevice(string deviceIdentifier)
        {
            return Devices.Activate(WebsocketConnection.SessionId, deviceIdentifier);
        }

        private IObservable<Unit> FinalizeStateChangeComplete(OmniServiceStatusEnum newState)
        {
            State = newState;
            Interlocked.Decrement(ref _migrationState);

            return Observable.Return(new Unit());
        }

                public void OnConfigurationChanged(ProxyConfiguration proxyConfiguration)
        {
            RestartIfStarted();
        }

        
        private IObservable<string> OpenWebsocketConnection()
        {
            WebsocketConnection = WebsocketConnectionFactory.Create();
            return WebsocketConnection.Connect();
        }

        private IObservable<Device> RegisterDevice()
        {
            var deviceIdentifier = ConfigurationService.DeviceIdentifier;
            var machineName = ConfigurationService.MachineName;

            GlobalConfig.AdditionalData.Add(new KeyValuePair<string, string>("Device Identifier", deviceIdentifier));

            return Devices.Create(deviceIdentifier, machineName);
        }

        private IObservable<Unit> StartCore()
        {
            return
                OpenWebsocketConnection()
                    .Select(_ => RegisterDevice())
                    .Switch()
                    .Select(device => ActivateDevice(device.Identifier))
                    .Switch()
                    .Select(_ => StartHandlers())
                    .Switch()
                    .Select(_ => StartMonitoringWebSocket())
                    .Switch()
                    .Take(1, SchedulerProvider.Default);
        }

        private IObservable<Unit> StartHandlers()
        {
            foreach (var handler in Kernel.GetAll<IHandler>() ?? Enumerable.Empty<IHandler>())
            {
                handler.Start(WebsocketConnection);
            }

            return Observable.Return(new Unit());
        }

        private IObservable<Unit> StartMonitoringWebSocket()
        {
            WebSocketMonitor.Stop();
            WebSocketMonitor.Start(WebsocketConnection);

            return Observable.Return(new Unit());
        }

        private IObservable<Unit> StopCore()
        {
            StopHandlers();
            WebsocketConnection.Disconnect();

            return Observable.Return(new Unit());
        }

        private void StopHandlers()
        {
            foreach (var handler in Kernel.GetAll<IHandler>() ?? Enumerable.Empty<IHandler>())
            {
                handler.Stop();
            }
        }

        private IObservable<Unit> SwitchToState(OmniServiceStatusEnum newState)
        {
            IObservable<Unit> result;
            if (_migrationState == NoSwitchInProgress)
            {
                Interlocked.Increment(ref _migrationState);
                var observable = newState == OmniServiceStatusEnum.Started ? StartCore() : StopCore();
                result = observable.Select(_ => FinalizeStateChangeComplete(newState)).Switch();
            }
            else
            {
                result = newState != State ? Observable.Empty<Unit>() : Observable.Return(new Unit());
            }

            return result;
        }

        #endregion
    }
}