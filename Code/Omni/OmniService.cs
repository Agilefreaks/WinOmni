namespace Omni
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading;
    using BugFreak;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using OmniSync;
    using Timer = System.Timers.Timer;

    public class OmniService : IOmniService
    {
        #region Fields

        protected readonly IObservable<ServiceStatusEnum> StatusChanged;

        private readonly IConfigurationService _configurationService;

        private readonly Timer _retryConnectionTimer = new Timer(5000) { AutoReset = true };

        private readonly AutoResetEvent _executing = new AutoResetEvent(true);

        private ServiceStatusEnum _status = ServiceStatusEnum.Stopped;

        private IDisposable _websocketConnectionObserver;

        #endregion

        #region Constructors and Destructors

        public OmniService(
            IConfigurationService configurationService,
            IWebsocketConnectionFactory websocketConnectionFactory)
        {
            WebsocketConnectionFactory = websocketConnectionFactory;
            _configurationService = configurationService;

            _retryConnectionTimer.Elapsed += (sender, arguments) => Start().Subscribe(_ => { }, _ => { });

            StatusChanged =
                Observable.FromEventPattern<ServiceStatusEventArgs>(
                    x => ConnectivityChanged += x,
                    x => ConnectivityChanged -= x).Select(x => x.EventArgs.Status);
        }

        #endregion

        #region Public Events

        public event EventHandler<ServiceStatusEventArgs> ConnectivityChanged;

        #endregion

        #region Public Properties

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
                if (_status != value)
                {
                    _status = value;

                    if (ConnectivityChanged != null)
                    {
                        ConnectivityChanged(this, new ServiceStatusEventArgs(_status));
                    }
                }
            }
        }

        public IObservable<ServiceStatusEnum> StatusChangedObservable
        {
            get
            {
                return StatusChanged;
            }
        }

        public IWebsocketConnectionFactory WebsocketConnectionFactory { get; set; }

        #endregion

        #region Properties

        protected IWebsocketConnection WebsocketConnection { get; set; }

        #endregion

        #region Public Methods and Operators

        public IObservable<Device> Start()
        {
            IObservable<Device> result = Observable.Empty<Device>();

            if (Status != ServiceStatusEnum.Started)
            {
                _executing.WaitOne();

                _executing.Reset();

                result = OpenWebsocketConnection()
                    .SelectMany(
                        registrationId =>
                            RegisterDevice()
                                .SelectMany(
                                    d => ActivateDevice(registrationId, d.Identifier).Select(
                                        device =>
                                        {
                                            _retryConnectionTimer.Stop();
                                            Status = ServiceStatusEnum.Started;

                                            RegisterConnectionObserver();
                                            StartHandlers();
                                            return device;
                                        }))).Finally(() => _executing.Set());
            }

            return result;
        }

        public void Stop(bool unsubscribeHandlers = true)
        {
            if (Status != ServiceStatusEnum.Started)
            {
                return;
            }

            Status = ServiceStatusEnum.Stopping;

            if (unsubscribeHandlers)
            {
                StopHandlers();
                WebsocketConnection.Disconnect();
            }

            Status = ServiceStatusEnum.Stopped;
        }

        #endregion

        #region Methods

        private IObservable<Device> ActivateDevice(string registrationId, string deviceIdentifier)
        {
            return Devices.Activate(registrationId, deviceIdentifier);
        }

        private void OnWebsocketConnectionLost()
        {
            if (Status == ServiceStatusEnum.Started)
            {
                Status = ServiceStatusEnum.Reconnecting;
                _retryConnectionTimer.Start();
            }

            Stop(false);
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
                    .Subscribe<WebsocketConnectionStatusEnum>(x => OnWebsocketConnectionLost(), _ => {});
        }

        private IObservable<Device> RegisterDevice()
        {
            var deviceIdentifier = _configurationService.DeviceIdentifier;
            var machineName = _configurationService.MachineName;
            
            GlobalConfig.AdditionalData.Add(new KeyValuePair<string, string>("Device Identifier", deviceIdentifier));
            
            return Devices.Create(deviceIdentifier, machineName);
        }

        private void StartHandlers()
        {
            foreach (var handler in Kernel.GetAll<IHandler>() ?? Enumerable.Empty<IHandler>())
            {
                handler.Start(WebsocketConnection);
            }
        }

        private void StopHandlers()
        {
            foreach (var handler in Kernel.GetAll<IHandler>() ?? Enumerable.Empty<IHandler>())
            {
                handler.Stop();
            }
        }

        #endregion
    }
}