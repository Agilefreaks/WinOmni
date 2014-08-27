namespace Omnipaste.Shell.Connection
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using Omni;
    using Omnipaste.Services.Connectivity;
    using Omnipaste.Services.SystemService;
    using OmniSync;

    public class ConnectionViewModel : Screen, IConnectionViewModel
    {
        #region Fields

        private IConnectivityNotifyService _connectivityNotifyService;

        private bool _enabled = true;

        private IOmniService _omniService;

        private IDisposable _omniServiceStatusObserver;

        private ISystemService _systemService;

        #endregion

        #region Constructors and Destructors

        public ConnectionViewModel(IOmniService omniService)
        {
            OmniService = omniService;
        }

        #endregion

        #region Public Properties

        public bool CanConnect
        {
            get
            {
                return !Connected;
            }
        }

        public bool CanDisconnect
        {
            get
            {
                return Connected;
            }
        }

        public bool Connected
        {
            get
            {
                return OmniService.Status == ServiceStatusEnum.Started;
            }
        }

        [Inject]
        public IConnectivityNotifyService ConnectivityNotifyService
        {
            get
            {
                return _connectivityNotifyService;
            }
            set
            {
                if (value == _connectivityNotifyService)
                {
                    return;
                }
                if (_connectivityNotifyService != null)
                {
                    _connectivityNotifyService.ConnectivityChanged -= ConnectivityChanged;
                }

                _connectivityNotifyService = value;
                _connectivityNotifyService.ConnectivityChanged += ConnectivityChanged;

                NotifyOfPropertyChange(() => ConnectivityNotifyService);
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                NotifyOfPropertyChange(() => Enabled);
            }
        }

        public IEventAggregator EventAggregator { get; set; }

        public bool IsConnected
        {
            get
            {
                return OmniService.Status == ServiceStatusEnum.Started;
            }
        }

        public IOmniService OmniService
        {
            get
            {
                return _omniService;
            }
            set
            {
                if (_omniServiceStatusObserver != null)
                {
                    _omniServiceStatusObserver.Dispose();
                }

                _omniService = value;

                _omniServiceStatusObserver = _omniService.StatusChangedObservable.Subscribe(
                    x =>
                    {
                        NotifyOfPropertyChange(() => CanConnect);
                        NotifyOfPropertyChange(() => CanDisconnect);
                    });
            }
        }

        [Inject]
        public ISystemService SystemService
        {
            get
            {
                return _systemService;
            }
            set
            {
                if (Equals(value, _systemService))
                {
                    return;
                }

                if (_systemService != null)
                {
                    _systemService.Resume -= SystemResumed;
                }

                _systemService = value;
                _systemService.Resume += SystemResumed;
                NotifyOfPropertyChange(() => SystemService);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Connect()
        {
            Enabled = false;
            OmniService.Start().Subscribe(device => Enabled = true);
        }

        public void Disconnect()
        {
            Enabled = false;

            OmniService.Stop();

            Enabled = true;
        }

        #endregion

        #region Methods

        private void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (!e.IsConnected)
            {
                Disconnect();
            }
            else if (e.IsConnected)
            {
                Connect();
            }
        }

        private void SystemResumed(object sender, EventArgs e)
        {
            Reconnect();
        }

        private void Reconnect()
        {
            Disconnect();
            Connect();
        }

        #endregion
    }
}