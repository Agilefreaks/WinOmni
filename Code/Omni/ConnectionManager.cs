namespace Omni
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using BugFreak;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniSync;

    public class ConnectionManager : IConnectionManager
    {
        #region Fields

        private readonly IObservable<long> _retryObservable;

        private bool _inTransition;

        private IDisposable _reconnectObserver;

        private IDisposable _websocketConnectionObserver;

        protected IWebsocketConnection WebsocketConnection;

        private readonly ReplaySubject<ServiceStatusEnum> _statusChangedSubject;

        private ServiceStatusEnum _status;

        #endregion

        #region Constructors and Destructors

        public ConnectionManager()
        {
            _status = ServiceStatusEnum.Stopped;
            _retryObservable = Observable.Timer(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            _statusChangedSubject = new ReplaySubject<ServiceStatusEnum>(0);
        }

        #endregion

        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IDevices Devices { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        public ServiceStatusEnum Status
        {
            get
            {
                return _status;
            }
            private set
            {
                _status = value;
                _statusChangedSubject.OnNext(value);
            }
        }

        public IObservable<ServiceStatusEnum> StatusChangedObservable
        {
            get
            {
                return _statusChangedSubject;
            }
        }

        [Inject]
        public IWebsocketConnectionFactory WebsocketConnectionFactory { get; set; }

        #endregion

        #region Public Methods and Operators

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            ReportingService.Instance.BeginReport(error);
        }

        public void OnNext(ServiceStatusEnum value)
        {
            GoToState(value)
                .SubscribeOn(SchedulerProvider.Default)
                .ObserveOn(SchedulerProvider.Default)
                .Subscribe(_ => { }, _ => { });
        }

        public IObservable<Unit> GoToState(ServiceStatusEnum newState)
        {
            return Status == newState || _inTransition
                       ? Observable.Empty<Unit>()
                       : MoveToNewState(newState);
        }

        #endregion

        #region Methods

        private IObservable<Device> ActivateDevice(string registrationId, string deviceIdentifier)
        {
            return Devices.Activate(registrationId, deviceIdentifier);
        }

        private void DisposeReconnectObserver()
        {
            if (_reconnectObserver != null)
            {
                _reconnectObserver.Dispose();
            }
        }
        
        private IObservable<Unit> MoveToNewState(ServiceStatusEnum value)
        {
            _inTransition = true;
            IObservable<Unit> observable;
            switch (value)
            {
                case ServiceStatusEnum.Started:
                    observable = Start();
                    break;
                case ServiceStatusEnum.Stopped:
                    observable = Stop();
                    break;
                case ServiceStatusEnum.Reconnecting:
                    observable = Reconnect();
                    break;
                default:
                    observable = Observable.Empty<Unit>();
                    break;
            }

            observable.SubscribeOn(SchedulerProvider.Default).ObserveOn(SchedulerProvider.Default).Subscribe(
                _ => OnStateChangeComplete(value),
                _ => { });

            return observable;
        }

        private IObservable<string> OpenWebsocketConnection()
        {
            WebsocketConnection = WebsocketConnectionFactory.Create();
            return WebsocketConnection.Connect();
        }

        private void RegisterConnectionObserver()
        {
            if (_websocketConnectionObserver != null)
            {
                _websocketConnectionObserver.Dispose();
            }

            _websocketConnectionObserver =
                WebsocketConnection.Where<WebsocketConnectionStatusEnum>(
                    x => x == WebsocketConnectionStatusEnum.Disconnected)
                    .Select(status => Observable.Return(ServiceStatusEnum.Reconnecting))
                    .Switch()
                    .SubscribeOn(Scheduler.Default)
                    .ObserveOn(Scheduler.Default)
                    .Subscribe(this);
        }

        private IObservable<Device> RegisterDevice()
        {
            var deviceIdentifier = ConfigurationService.DeviceIdentifier;
            var machineName = ConfigurationService.MachineName;

            GlobalConfig.AdditionalData.Add(new KeyValuePair<string, string>("Device Identifier", deviceIdentifier));

            return Devices.Create(deviceIdentifier, machineName);
        }

        private IObservable<Unit> Start()
        {
            var subject = new ReplaySubject<Unit>(0);
            var activateDevice =
                OpenWebsocketConnection()
                    .Select(
                        registrationId =>
                        RegisterDevice().Select(device => ActivateDevice(registrationId, device.Identifier)).Switch())
                    .Switch()
                    .Take(1, SchedulerProvider.Default);
            activateDevice.Select(device => new Unit())
                .SubscribeOn(SchedulerProvider.Default)                
                .ObserveOn(SchedulerProvider.Default)
                .Subscribe(subject);
            activateDevice.SubscribeOn(SchedulerProvider.Default)
                .ObserveOn(SchedulerProvider.Default)
                .Subscribe(OnActivatedDevice, _ => { });

            return subject;
        }

        private void OnActivatedDevice(Device device)
        {
            DisposeReconnectObserver();
            RegisterConnectionObserver();
            StartHandlers();
        }

        private IObservable<Unit> Stop()
        {
            switch (Status)
            {
                case ServiceStatusEnum.Reconnecting:
                    DisposeReconnectObserver();
                    break;
                case ServiceStatusEnum.Started:
                    StopHandlers();
                    WebsocketConnection.Disconnect();
                    break;
                case ServiceStatusEnum.Stopped:
                    break;
                default:
                    ReportingService.Instance.BeginReport(new Exception("Unknown newState: " + Status));
                    break;
            }

            return Observable.Return(new Unit());
        }

        private IObservable<Unit> Reconnect()
        {
            if (Status == ServiceStatusEnum.Started)
            {
                Stop();
                DisposeReconnectObserver();
                _reconnectObserver =
                    _retryObservable.SubscribeOn(Scheduler.Default)
                        .ObserveOn(Scheduler.Default)
                        .Select(_ => Observable.Return(ServiceStatusEnum.Started))
                        .Switch()
                        .Subscribe(this);
            }

            return Observable.Return(new Unit());
        }

        private void StopHandlers()
        {
            foreach (var handler in Kernel.GetAll<IHandler>() ?? Enumerable.Empty<IHandler>())
            {
                handler.Stop();
            }
        }

        private void StartHandlers()
        {
            foreach (var handler in Kernel.GetAll<IHandler>() ?? Enumerable.Empty<IHandler>())
            {
                handler.Start(WebsocketConnection);
            }
        }

        private void OnStateChangeComplete(ServiceStatusEnum newState)
        {
            Status = newState;
            _inTransition = false;
        }

        #endregion
    }
}