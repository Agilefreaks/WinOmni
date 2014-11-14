namespace Omni
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
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

        private readonly ReplaySubject<bool> _inTransitionChangedSubject;

        private readonly ReplaySubject<OmniServiceStatusEnum> _statusChangedSubject;

        private int _migrationState;

        private OmniServiceStatusEnum _state;

        #endregion

        #region Constructors and Destructors

        public OmniService()
        {
            _statusChangedSubject = new ReplaySubject<OmniServiceStatusEnum>(1);
            _inTransitionChangedSubject = new ReplaySubject<bool>(1);
            State = OmniServiceStatusEnum.Stopped;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IDevices Devices { get; set; }

        public bool InTransition
        {
            get
            {
                return _migrationState != NoSwitchInProgress;
            }
        }

        public IObservable<bool> InTransitionObservable
        {
            get
            {
                return _inTransitionChangedSubject;
            }
        }

        [Inject]
        public IKernel Kernel { get; set; }

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

        private void AssureAuthenticationCredentialsExist()
        {
            if (string.IsNullOrWhiteSpace(ConfigurationService.AccessToken))
            {
                throw new Exception("No authentication credentials exist");
            }
        }

        private void FinalizeStateChangeComplete(OmniServiceStatusEnum newState)
        {
            State = newState;
            ReleaseApplicationStateLock();
        }

        private void LockApplicationState()
        {
            Interlocked.Increment(ref _migrationState);
            _inTransitionChangedSubject.OnNext(InTransition);
        }

        private IObservable<Unit> OnStateTransitionException(Exception exception)
        {
            ReleaseApplicationStateLock();

            return Observable.Throw<Unit>(exception);
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

        private void ReleaseApplicationStateLock()
        {
            Interlocked.Decrement(ref _migrationState);
            _inTransitionChangedSubject.OnNext(InTransition);
        }

        private IObservable<Unit> StartCore()
        {
            return
                Observable.Start(AssureAuthenticationCredentialsExist, SchedulerProvider.Default)
                    .Select(_ => OpenWebsocketConnection())
                    .Switch()
                    .Select(_ => RegisterDevice())
                    .Switch()
                    .Select(device => ActivateDevice(device.Identifier))
                    .Switch()
                    .Select(_ => Observable.Start(StartHandlers, SchedulerProvider.Default))
                    .Switch()
                    .Select(_ => Observable.Start(StartMonitoringWebSocket, SchedulerProvider.Default))
                    .Switch();
        }

        private void StartHandlers()
        {
            foreach (var handler in Kernel.GetAll<IHandler>() ?? Enumerable.Empty<IHandler>())
            {
                handler.Start(WebsocketConnection);
            }
        }

        private void StartMonitoringWebSocket()
        {
            WebSocketMonitor.Stop();
            WebSocketMonitor.Start(WebsocketConnection);
        }

        private IObservable<Unit> StopCore()
        {
            return Observable.Start(
                () =>
                    {
                        StopHandlers();
                        WebsocketConnection.Disconnect();
                    },
                SchedulerProvider.Default);
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
            if (newState == State)
            {
                result = Observable.Return(new Unit(), SchedulerProvider.Default);
            }
            else if (_migrationState == NoSwitchInProgress)
            {
                LockApplicationState();
                var observable = newState == OmniServiceStatusEnum.Started ? StartCore() : StopCore();
                result =
                    observable.Select(
                        _ => Observable.Start(() => FinalizeStateChangeComplete(newState), SchedulerProvider.Default))
                        .Switch()
                        .Catch<Unit, Exception>(OnStateTransitionException);
            }
            else
            {
                result = Observable.Throw<Unit>(new Exception("Transition in progress"), SchedulerProvider.Default);
            }

            return result;
        }

        #endregion
    }
}