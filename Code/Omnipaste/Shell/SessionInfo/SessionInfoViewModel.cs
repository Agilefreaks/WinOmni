namespace Omnipaste.Shell.SessionInfo
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Omni;
    using OmniCommon;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class SessionInfoViewModel : Screen, ISessionInfoViewModel
    {
        #region Fields

        private readonly Dictionary<ConnectionStateEnum, string> _icons;

        private readonly Dictionary<ConnectionStateEnum, string> _statusTexts;

        private readonly IDisposable _statusObserver;

        private readonly IDisposable _userObserver;

        private ConnectionStateEnum _state;
        
        private UserInfo _userInfo;

        #endregion

        #region Constructors and Destructors

        public SessionInfoViewModel(IOmniService omniService, IConfigurationService configurationService)
        {
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
            
            UserInfo = configurationService.UserInfo;
            _userObserver =
                configurationService.SettingsChangedObservable.SubscribeToSettingChange<UserInfo>(
                    ConfigurationProperties.UserInfo,
                    userInfo => UserInfo = userInfo,
                    observeScheduler: SchedulerProvider.Dispatcher);
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

        public string UserName
        {
            get
            {
                return UserInfo.FullName();
            }
        }

        public string UserImage
        {
            get
            {
                return UserInfo.ImageUrl;
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

        public UserInfo UserInfo
        {
            get
            {
                return _userInfo;
            }
            set
            {
                _userInfo = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => UserImage);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _statusObserver.Dispose();
            _userObserver.Dispose();
        }

        #endregion
    }
}
