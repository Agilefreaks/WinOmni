namespace Omni
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Timers;
    using Clipboard;
    using Events;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using OmniSync;

    public class OmniService : IOmniService
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly Timer _retryConnectionTimer = new Timer(5000) { AutoReset = true };

        private readonly IObservable<ServiceStatusEnum> _statusChanged;

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

            _retryConnectionTimer.Elapsed += (sender, args) => Start().Subscribe();

            _statusChanged =
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

        public IWebsocketConnectionFactory WebsocketConnectionFactory { get; set; }

        #endregion

        #region Properties

        private IWebsocketConnection WebsocketConnection { get; set; }

        #endregion

        #region Public Methods and Operators

        public IObservable<Device> Start()
        {
            if (Status == ServiceStatusEnum.Started)
            {
                Observable.Empty<Device>();
            }

            return OpenWebsocketConnection()
                .SelectMany(registrationId => RegisterDevice()
                    .SelectMany(d => ActivateDevice(registrationId, d.Identifier)
                        .Select(
                            device =>
                            {
                                _retryConnectionTimer.Stop();
                                Status = ServiceStatusEnum.Started;

                                RegisterConnectionObserver();
                                StartHandlers();
                                return device;
                            })));
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
                StopHandlders();
                WebsocketConnection.Disconnect();
            }

            Status = ServiceStatusEnum.Stopped;
        }

        public IDisposable Subscribe(IObserver<ServiceStatusEnum> observer)
        {
            return _statusChanged.Subscribe(observer);
        }

        #endregion

        #region Methods

        private IObservable<string> OpenWebsocketConnection()
        {
            WebsocketConnection = WebsocketConnectionFactory.Create();
            return WebsocketConnection.Connect();
        }

        private IObservable<Device> RegisterDevice()
        {
            var deviceIdentifier = _configurationService.DeviceIdentifier;
            var machineName = _configurationService.MachineName;

            return Devices.Create(deviceIdentifier, machineName);
        }

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

        private void RegisterConnectionObserver()
        {
            if (_websocketConnectionObserver != null)
            {
                _websocketConnectionObserver.Dispose();
            }

            _websocketConnectionObserver =
                WebsocketConnection.Where<WebsocketConnectionStatusEnum>(
                    x => x == WebsocketConnectionStatusEnum.Disconnected)
                    .Subscribe<WebsocketConnectionStatusEnum>(x => OnWebsocketConnectionLost());
        }

        private void StartHandlers()
        {
            if (Kernel.GetModules().All(m => m.GetType() != typeof(ClipboardModule)))
            {
                Kernel.Load(new ClipboardModule());
            }

            if (Kernel.GetModules().All(m => m.GetType() != typeof(EventsModule)))
            {
                Kernel.Load(new EventsModule());
            }

            foreach (var handler in Kernel.GetAll<IHandler>() ?? Enumerable.Empty<IHandler>())
            {
                handler.Start(WebsocketConnection);
            }
        }

        private void StopHandlders()
        {
            foreach (var handler in Kernel.GetAll<IHandler>() ?? Enumerable.Empty<IHandler>())
            {
                handler.Stop();
            }
        }

        #endregion
    }
}