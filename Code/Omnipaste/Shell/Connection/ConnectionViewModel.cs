namespace Omnipaste.Shell.Connection
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Microsoft.Win32;
    using Omni;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.ExtensionMethods;
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

        private readonly Dictionary<ConnectionStateEnum, string> _icons;

        private readonly Dictionary<ConnectionStateEnum, string> _toolTips;

        private bool _canPerformAction = true;

        private IOmniService _omniService;

        private IDisposable _omniServiceStatusObserver;

        private ConnectionStateEnum _state;

        #endregion

        #region Constructors and Destructors

        public ConnectionViewModel(
            IOmniService omniService,
            ISystemService systemService,
            IConnectivityNotifyService connectivityNotifyService)
        {
            _connectivityNotifyService = connectivityNotifyService;
            connectivityNotifyService.ConnectivityChangedObservable.SubscribeOn(Scheduler.Default)
                .ObserveOn(SchedulerProvider.Dispatcher)
                .SubscribeAndHandleErrors(OnConnectivityChanged);
            systemService.PowerModesObservable.SubscribeAndHandleErrors(HandleNewPowerMode);
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
                NotifyOfPropertyChange(() => IsConnected);
            }
        }

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
                return State == ConnectionStateEnum.Connected;
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
                        newState =>
                        State =
                        newState == ServiceStatusEnum.Started
                            ? ConnectionStateEnum.Connected
                            : ConnectionStateEnum.Disconnected);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Connect()
        {
            CanPerformAction = false;
            OmniService.StartWithDefaultObserver();
            CanPerformAction = true;
        }

        public void Disconnect()
        {
            CanPerformAction = false;

            OmniService.StopWithDefaultObserver();

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

        private void HandleNewPowerMode(PowerModes systemPowerMode)
        {
            switch (systemPowerMode)
            {
                case PowerModes.Resume:
                    SystemResumed();
                    break;
                case PowerModes.Suspend:
                    SystemSuspended();
                    break;
            }
        }

        private void SystemResumed()
        {
            if (_connectivityNotifyService.CurrentlyConnected)
            {
                Connect();
            }
        }

        private void SystemSuspended()
        {
            Disconnect();
        }

        #endregion
    }
}