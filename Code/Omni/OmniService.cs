namespace Omni
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Timers;
    using Clipboard;
    using Ninject;
    using Notifications;
    using OmniApi.Models;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using OmniSync;
    using RestSharp;

    public class OmniService : IOmniService
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly Timer _retryConnectionTimer = new Timer(5000) { AutoReset = true };

        private readonly IObservable<ServiceStatusEnum> _statusChanged;

        private IDevicesApi _devicesApi;

        private ServiceStatusEnum _status = ServiceStatusEnum.Stopped;

        private IWebsocketConnection _websocketConnection;

        private IDisposable _websocketConnectionObserver;

        #endregion

        #region Constructors and Destructors

        public OmniService(
            IConfigurationService configurationService,
            IWebsocketConnectionFactory websocketConnectionFactory)
        {
            WebsocketConnectionFactory = websocketConnectionFactory;
            _configurationService = configurationService;

            _retryConnectionTimer.Elapsed += Reconnect;

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

        private IWebsocketConnection WebsocketConnection
        {
            get
            {
                return _websocketConnection;
            }
            set
            {
                if (_websocketConnectionObserver != null)
                {
                    _websocketConnectionObserver.Dispose();
                }

                _websocketConnection = value;

                if (_websocketConnection != null)
                {
                    _websocketConnectionObserver =
                        _websocketConnection.Where<WebsocketConnectionStatusEnum>(
                            x => x == WebsocketConnectionStatusEnum.Disconnected)
                            .Subscribe<WebsocketConnectionStatusEnum>(x => OnWebsocketConnectionLost());
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public async Task Start(string communicationChannel = null)
        {
            if (Status == ServiceStatusEnum.Started)
            {
                return;
            }

            await OpenWebsocketConnection();

            if (WebsocketConnection.RegistrationId != null)
            {
                _devicesApi = Kernel.Get<IDevicesApi>();

                var deviceIdentifier = await RegisterDevice();

                var activationResult = await ActivateDevice(WebsocketConnection.RegistrationId, deviceIdentifier);

                if (activationResult.Data != null)
                {
                    Status = ServiceStatusEnum.Started;

                    StartHandlers();
                }
            }
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

        private async Task OpenWebsocketConnection()
        {
            WebsocketConnection = WebsocketConnectionFactory.Create();
            await WebsocketConnection.Connect();
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

        private async void Reconnect(object sender, ElapsedEventArgs e)
        {
            await Start();

            if (Status == ServiceStatusEnum.Started)
            {
                _retryConnectionTimer.Stop();
            }
        }

        private async Task<string> RegisterDevice()
        {
            var deviceIdentifier = _configurationService.DeviceIdentifier;
            var machineName = _configurationService.MachineName;

            await _devicesApi.Register(deviceIdentifier, machineName);
            return deviceIdentifier;
        }

        private async Task<IRestResponse<Device>> ActivateDevice(string registrationId, string deviceIdentifier)
        {
            const string NotificationProvider = "omni_sync";
            var activationResult = await _devicesApi.Activate(registrationId, deviceIdentifier, NotificationProvider);
            return activationResult;
        }

        private void StartHandlers()
        {
            if (Kernel.GetModules().All(m => m.GetType() != typeof(ClipboardModule)))
            {
                Kernel.Load(new ClipboardModule());
            }

            if (Kernel.GetModules().All(m => m.GetType() != typeof(NotificationsModule)))
            {
                Kernel.Load(new NotificationsModule());
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