﻿namespace Omni
{
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using Ninject;
    using OmniApi.Dto;
    using OmniApi.Resources.v1;
    using OmniCommon;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniSync;

    public class OmniService : IOmniService
    {
        #region Constants

        private const int NoSwitchInProgress = 0;
        private const int SwitchInProgress = 1;

        #endregion

        #region Fields

        protected IWebsocketConnection WebsocketConnection;

        private int _migrationState = NoSwitchInProgress;

        private ISubject<OmniServiceStatusEnum> _statusChangedSubject;

        private OmniServiceStatusEnum _state;

        private bool _isDisposing;

        private readonly Subject<bool> _inTransitionSubject;

        #endregion

        #region Constructors and Destructors

        public OmniService()
        {
            _statusChangedSubject = new ReplaySubject<OmniServiceStatusEnum>(1);
            _inTransitionSubject = new Subject<bool>();
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
                return _inTransitionSubject;
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

        public void Dispose()
        {
            _isDisposing = true;
            if (InTransition)
            {
                InTransitionObservable.Take(1).Wait();
            }

            _statusChangedSubject = new NullSubject<OmniServiceStatusEnum>();

            if (State != OmniServiceStatusEnum.Stopped)
            {
                StopCore().RunToCompletionSynchronous();
            }
        }

        #endregion

        #region Methods

        private static IObservable<Unit> OnDeactivateDeviceException(Exception exception)
        {
            SimpleLogger.Log("Could not deactivate device: " + exception);
            ExceptionReporter.Instance.Report(exception);
            return Observable.Return(new Unit(), SchedulerProvider.Default);
        }

        private IObservable<EmptyDto> ActivateDevice(string deviceId)
        {
            SimpleLogger.Log("Activating device");
            return Devices.Activate(WebsocketConnection.SessionId, deviceId);
        }

        private void AssureAuthenticationCredentialsExist()
        {
            SimpleLogger.Log("Checking for credentials");
            if (string.IsNullOrWhiteSpace(ConfigurationService.AccessToken))
            {
                throw new Exception("No authentication credentials exist");
            }
        }

        private void FinalizeStateChangeComplete(OmniServiceStatusEnum newState)
        {
            SimpleLogger.Log("Finalizing state change");
            State = newState;
            ReleaseServiceState();
        }

        private bool LockServiceState()
        {
            var result = false;
            SimpleLogger.Log("Trying to lock service state");
            var previousValue = Interlocked.Exchange(ref _migrationState, SwitchInProgress);
            if (previousValue == NoSwitchInProgress)
            {
                SimpleLogger.Log("No lock already in place; returning true");

                result = true;
                _inTransitionSubject.OnNext(InTransition);
            }
            else
            {
                SimpleLogger.Log("Lock already in place; returning false");
            }

            return result;
        }

        private IObservable<Unit> OnStateTransitionException(Exception exception)
        {
            SimpleLogger.Log("State transition exception: " + exception);
            ReleaseServiceState();

            return Observable.Throw<Unit>(exception);
        }

        private IObservable<string> OpenWebsocketConnection()
        {
            SimpleLogger.Log("Opening websocket connection");
            WebsocketConnection = WebsocketConnectionFactory.Create();
            return WebsocketConnection.Connect();
        }

        private void ReleaseServiceState()
        {
            SimpleLogger.Log("Release service state");
            Interlocked.Exchange(ref _migrationState, NoSwitchInProgress);
            _inTransitionSubject.OnNext(InTransition);
        }

        private IObservable<Unit> StartCore()
        {
            SimpleLogger.Log("Call to start core");
            return
                Observable.Start(AssureAuthenticationCredentialsExist, SchedulerProvider.Default)
                    .Select(_ => OpenWebsocketConnection())
                    .Switch()
                    .Select(_ => ActivateDevice(ConfigurationService.DeviceId))
                    .Switch()
                    .Select(_ => Observable.Start(StartHandlers, SchedulerProvider.Default))
                    .Switch()
                    .Select(_ => Observable.Start(StartMonitoringWebSocket, SchedulerProvider.Default))
                    .Switch();
        }

        private void StartHandlers()
        {
            SimpleLogger.Log("Starting handlers");
            foreach (var handler in Kernel.GetAll<IHandler>() ?? Enumerable.Empty<IHandler>())
            {
                handler.Start(WebsocketConnection);
            }
        }

        private void StartMonitoringWebSocket()
        {
            SimpleLogger.Log("Starting websocket monitor");
            WebSocketMonitor.Stop();
            WebSocketMonitor.Start(WebsocketConnection);
        }

        private IObservable<Unit> StopCore()
        {
            return
                Observable.Start(StopHandlers, SchedulerProvider.Default)
                    .Select(_ => Observable.Start(WebsocketConnection.Disconnect, SchedulerProvider.Default))
                    .Switch()
                    .Select(_ => DeactivateDevice())
                    .Switch();
        }

        private IObservable<Unit> DeactivateDevice()
        {
            SimpleLogger.Log("Deactivating device");
            return
                Devices.Deactivate(ConfigurationService.DeviceId)
                    .Select(_ => new Unit())
                    .Catch<Unit, Exception>(OnDeactivateDeviceException);
        }

        private void StopHandlers()
        {
            SimpleLogger.Log("Stopping handlers");
            foreach (var handler in Kernel.GetAll<IHandler>() ?? Enumerable.Empty<IHandler>())
            {
                handler.Stop();
            }
            SimpleLogger.Log("Stopped handlers");
        }

        private IObservable<Unit> SwitchToState(OmniServiceStatusEnum newState)
        {
            SimpleLogger.Log("Call to switch state with: " + newState);
            IObservable<Unit> result;
            
            if (_isDisposing)
            {
                SimpleLogger.Log("OmniService is disposing.");
                result = Observable.Throw<Unit>(new Exception("Disposing"), SchedulerProvider.Default);
            }
            else if (newState == State)
            {
                SimpleLogger.Log("Same state found; doing nothing");
                result = Observable.Return(new Unit(), SchedulerProvider.Default);
            }
            else if (LockServiceState())
            {
                SimpleLogger.Log("Acquired lock, switchhing state");
                var observable = newState == OmniServiceStatusEnum.Started ? StartCore() : StopCore();
                result =
                    observable.Select(
                        _ => Observable.Start(() => FinalizeStateChangeComplete(newState), SchedulerProvider.Default))
                        .Switch()
                        .Catch<Unit, Exception>(OnStateTransitionException);
            }
            else
            {
                SimpleLogger.Log("Could not acquire lock, returning failed result");
                result = Observable.Throw<Unit>(new Exception("Transition in progress"), SchedulerProvider.Default);
            }

            return result;
        }

        #endregion
    }
}