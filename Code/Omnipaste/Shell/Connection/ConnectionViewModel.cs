namespace Omnipaste.Shell.Connection
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Omni;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Properties;
    using OmniUI.Attributes;

    [UseView("OmniUI.HeaderButton.HeaderItemView", IsFullyQualifiedName = true)]
    public class ConnectionViewModel : Screen, IConnectionViewModel
    {
        #region Fields

        private readonly Dictionary<ConnectionStateEnum, string> _icons;

        private readonly Dictionary<ConnectionStateEnum, string> _toolTips;

        private readonly IDisposable _statusObserver;

        private ConnectionStateEnum _state;

        #endregion

        #region Constructors and Destructors

        public ConnectionViewModel(IOmniService omniService)
        {
            _toolTips = new Dictionary<ConnectionStateEnum, string>
                            {
                                { ConnectionStateEnum.Connected, Resources.Connected },
                                { ConnectionStateEnum.Disconnected, Resources.Disconnected }
                            };
            _icons = new Dictionary<ConnectionStateEnum, string>
                         {
                             { ConnectionStateEnum.Connected, Resources.DisconnectIcon },
                             { ConnectionStateEnum.Disconnected, Resources.ConnectIcon }
                         };
            _statusObserver =
                omniService.StatusChangedObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(
                        newStatus =>
                        State =
                        newStatus == OmniServiceStatusEnum.Started
                            ? ConnectionStateEnum.Connected
                            : ConnectionStateEnum.Disconnected);
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

        public string Icon
        {
            get
            {
                return _icons[State];
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

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _statusObserver.Dispose();
        }

        #endregion
    }
}