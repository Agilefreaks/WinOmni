﻿namespace Omni
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Timers;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using OmniSync;
    using RestSharp;

    public class OmniService : IOmniService
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly IDevicesApi _devicesApi;

        private readonly Timer _retryConnectionTimer = new Timer(5000) { AutoReset = true };

        private readonly IObservable<ServiceStatusEnum> _statusChanged;

        private ServiceStatusEnum _status = ServiceStatusEnum.Stopped;

        private IWebsocketConnection _websocketConnection;

        private IDisposable _websocketConnectionObserver;

        #endregion

        #region Constructors and Destructors

        public OmniService(
            IDevicesApi devicesApi,
            IConfigurationService configurationService,
            IWebsocketConnectionFactory websocketConnectionFactory)
        {
            WebsocketConnectionFactory = websocketConnectionFactory;
            _devicesApi = devicesApi;
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
        public IEnumerable<IOmniMessageHandler> MessageHandlers { get; set; }

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
                var deviceIdentifier = await RegisterDevice();

                var activationResult = await ActivateDevice(WebsocketConnection.RegistrationId, deviceIdentifier);

                if (activationResult.Data != null)
                {
                    Status = ServiceStatusEnum.Started;
                }
            }
        }

        public void Stop(bool unsubscribeHandlers = true)
        {
            if (Status != ServiceStatusEnum.Started)
            {
                return;
            }

            Status = ServiceStatusEnum.Stopped;

            if (unsubscribeHandlers)
            {
                UnsubscribeMessageHandlers();
                WebsocketConnection.Disconnect();
            }
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
            SubscribeMessageHandlers();
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

        private void SubscribeMessageHandlers()
        {
            foreach (var messageHandler in MessageHandlers)
            {
                messageHandler.SubscribeTo(WebsocketConnection);
            }
        }

        private void UnsubscribeMessageHandlers()
        {
            foreach (var messageHandler in MessageHandlers)
            {
                messageHandler.Dispose();
            }
        }

        #endregion
    }
}