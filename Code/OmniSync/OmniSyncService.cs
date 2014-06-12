namespace OmniSync
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using Ninject;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class OmniSyncService : IOmniSyncService, IObserver<WebsocketConnectionStatusEnum>
    {
        #region Fields

        private readonly IObservable<ServiceStatusEnum> _statusChanged;

        private readonly IWebsocketConnectionFactory _websocketConnectionFactory;

        private ISubject<OmniMessage> _omniMessageObservable;

        private ServiceStatusEnum _status;

        private IWebsocketConnection _websocketConnection;

        private IDisposable _websocketConnectionObserver;

        #endregion

        #region Constructors and Destructors

        public OmniSyncService(IWebsocketConnectionFactory websocketConnectionFactory)
        {
            _websocketConnectionFactory = websocketConnectionFactory;
            
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
                    _websocketConnectionObserver = _websocketConnection.Subscribe(this);
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(WebsocketConnectionStatusEnum value)
        {
            if (value == WebsocketConnectionStatusEnum.Disconnected)
            {
                Stop();
            }
        }

        public async Task<RegistrationResult> Start()
        {
            if (Status != ServiceStatusEnum.Stopped)
            {
                return new RegistrationResult { Data = WebsocketConnection.RegistrationId };
            }

            WebsocketConnection =
                _websocketConnectionFactory.Create(ConfigurationManager.AppSettings["OmniSyncUrl"]);
            _omniMessageObservable = await WebsocketConnection.Connect();

            SubscribeMessageHandlers();

            Status = ServiceStatusEnum.Started;

            return new RegistrationResult { Data = WebsocketConnection.RegistrationId };
        }

        public void Stop()
        {
            if (Status != ServiceStatusEnum.Started)
            {
                return;
            }

            UnsubscribeMessageHandlers();

            WebsocketConnection.Disconnect();

            Status = ServiceStatusEnum.Stopped;
        }

        public IDisposable Subscribe(IObserver<ServiceStatusEnum> observer)
        {
            return _statusChanged.Subscribe(observer);
        }

        #endregion

        #region Methods

        private void SubscribeMessageHandlers()
        {
            foreach (var messageHandler in MessageHandlers)
            {
                messageHandler.SubscribeTo(_omniMessageObservable);
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