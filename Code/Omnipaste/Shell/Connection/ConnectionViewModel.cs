namespace Omnipaste.Shell.Connection
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Ninject;
    using Omni;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Framework;
    using Omnipaste.Properties;
    using Omnipaste.Services.Connectivity;
    using Omnipaste.Services.SystemService;
    using OmniSync;
    using OmniUI.Attributes;

    [UseView("OmniUI.HeaderButton.HeaderButtonView", IsFullyQualifiedName = true)]
    public class ConnectionViewModel : Screen, IConnectionViewModel
    {
        #region Fields

        private readonly IConnectivityNotifyService _connectivityNotifyService;

        private bool _canPerformAction = true;

        private IOmniService _omniService;

        private IDisposable _omniServiceStatusObserver;

        private ISystemService _systemService;

        private ConnectionStateEnum _state;

        private readonly Dictionary<ConnectionStateEnum, string> _toolTips;
        private readonly Dictionary<ConnectionStateEnum, string> _icons;

        #endregion

        #region Constructors and Destructors

        public ConnectionViewModel(
            IOmniService omniService,
            IConnectivityNotifyService connectivityNotifyService)
        {
            _connectivityNotifyService = connectivityNotifyService;
            connectivityNotifyService.ConnectivityChangedObservable.SubscribeOn(Scheduler.Default)
                .ObserveOn(SchedulerProvider.Dispatcher)
                .SubscribeAndHandleErrors(OnConnectivityChanged);
            OmniService = omniService;
            _toolTips = new Dictionary<ConnectionStateEnum, string>
                            {
                                { ConnectionStateEnum.Connected, Resources.ConnectionDisconnect },
                                { ConnectionStateEnum.Disconnected, Resources.ConnectionConnect }
                            };
            _icons = new Dictionary<ConnectionStateEnum, string>
                         {
                             { ConnectionStateEnum.Connected, Resources.DisconnectIcon },
                             { ConnectionStateEnum.Disconnected, Resources.ConnectIcon }
                         };
        }

        #endregion

        #region Public Properties

        public string ButtonToolTip
        {
            get
            {
                return _toolTips[State];
            }
        }

        public bool CanPerformAction
        {
            get
            {
                return _canPerformAction;
            }
            set
            {
                _canPerformAction = value;
                NotifyOfPropertyChange();
            }
        }

        public ConnectionStateEnum State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => ButtonToolTip);
                NotifyOfPropertyChange(() => Icon);
            }
        }

        public IEventAggregator EventAggregator { get; set; }

        public string Icon
        {
            get
            {
                return _icons[State];
            }
        }

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

                _omniServiceStatusObserver =
                    _omniService.StatusChangedObservable.SubscribeAndHandleErrors(
                        x =>
                        State =
                        _omniService.Status == ServiceStatusEnum.Started
                            ? ConnectionStateEnum.Connected
                            : ConnectionStateEnum.Disconnected);
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
                    _systemService.Resumed -= SystemResumed;
                    _systemService.Suspended -= SystemSuspended;
                }

                _systemService = value;
                _systemService.Resumed += SystemResumed;
                _systemService.Suspended += SystemSuspended;
                NotifyOfPropertyChange(() => SystemService);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Connect()
        {
            CanPerformAction = false;
            OmniService.Start()
                .SubscribeOn(Scheduler.Default)
                .ObserveOn(SchedulerProvider.Dispatcher)
                .SubscribeAndHandleErrors(device => CanPerformAction = true);
        }

        public void Disconnect()
        {
            CanPerformAction = false;

            OmniService.Stop();

            CanPerformAction = true;
        }

        public void PerformAction()
        {
            if (State == ConnectionStateEnum.Connected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        #endregion

        #region Methods

        private void OnConnectivityChanged(bool currentlyConnected)
        {
            if (currentlyConnected)
            {
                Connect();
            }
            else
            {
                Disconnect();
            }
        }

        private void SystemResumed(object sender, EventArgs e)
        {
            if (ConnectivityNotifyService.CurrentlyConnected)
            {
                Connect();
            }
        }

        private void SystemSuspended(object sender, EventArgs e)
        {
            Disconnect();
        }

        #endregion
    }
}