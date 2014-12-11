using Ninject;
using Omnipaste.Services;

namespace Omnipaste.Shell.SessionInfo
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Omni;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;

    public class SessionInfoViewModel : Screen, ISessionInfoViewModel
    {
        private readonly ISessionManager _sessionManager;

        #region Fields

        private readonly Dictionary<ConnectionStateEnum, string> _icons;

        private readonly Dictionary<ConnectionStateEnum, string> _statusTexts;

        private readonly IDisposable _statusObserver;

        private ConnectionStateEnum _state;

        #endregion

        #region Constructors and Destructors

        public SessionInfoViewModel(IOmniService omniService, ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _statusTexts = new Dictionary<ConnectionStateEnum, string>
                            {
                                { ConnectionStateEnum.Connected, Properties.Resources.Connected },
                                { ConnectionStateEnum.Disconnected, Properties.Resources.Disconnected }
                            };
            _icons = new Dictionary<ConnectionStateEnum, string>
                         {
                             { ConnectionStateEnum.Connected, OmniUI.Resources.IconNames.Connected },
                             { ConnectionStateEnum.Disconnected, OmniUI.Resources.IconNames.Disconnected }
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

        public string StatusText
        {
            get
            {
                return _statusTexts[State];
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
                NotifyOfPropertyChange(() => StatusText);
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
