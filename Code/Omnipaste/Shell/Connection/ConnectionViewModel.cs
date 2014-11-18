namespace Omnipaste.Shell.Connection
{
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Omni;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Properties;
    using Omnipaste.Services.Monitors.User;
    using OmniUI.Attributes;

    [UseView("OmniUI.HeaderButton.HeaderButtonView", IsFullyQualifiedName = true)]
    public class ConnectionViewModel : Screen, IConnectionViewModel
    {
        #region Fields

        private readonly Dictionary<ConnectionStateEnum, string> _icons;

        private readonly Dictionary<ConnectionStateEnum, string> _toolTips;

        private readonly IUserMonitor _userMonitor;

        private bool _canPerformAction = true;

        private ConnectionStateEnum _state;

        #endregion

        #region Constructors and Destructors

        public ConnectionViewModel(IUserMonitor userMonitor, IOmniService omniService)
        {
            _userMonitor = userMonitor;
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
            omniService.StatusChangedObservable.SubscribeAndHandleErrors(
                newStatus =>
                State =
                newStatus == OmniServiceStatusEnum.Started
                    ? ConnectionStateEnum.Connected
                    : ConnectionStateEnum.Disconnected);
            omniService.InTransitionObservable.SubscribeAndHandleErrors(
                isInTransition => CanPerformAction = !isInTransition);
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
                NotifyOfPropertyChange(() => CanPerformAction);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void PerformAction()
        {
            _userMonitor.SendEvent(
                State == ConnectionStateEnum.Connected ? UserEventTypeEnum.Disconnect : UserEventTypeEnum.Connect);
        }

        #endregion
    }
}