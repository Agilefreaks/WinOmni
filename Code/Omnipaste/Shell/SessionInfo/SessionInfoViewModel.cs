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
    using Omnipaste.Models;
    using OmniUI.Presenters;

    public class SessionInfoViewModel : Screen, ISessionInfoViewModel
    {
        #region Fields

        private readonly Dictionary<ConnectionStateEnum, string> _icons;

        private readonly Dictionary<ConnectionStateEnum, string> _statusTexts;

        private readonly IDisposable _statusObserver;

        private readonly IDisposable _userObserver;

        private ConnectionStateEnum _state;

        private ContactInfoPresenter _userInfo;

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

            UpdateUserInfo(configurationService.UserInfo);
            _userObserver =
                configurationService.SettingsChangedObservable.SubscribeToSettingChange<UserInfo>(
                    ConfigurationProperties.UserInfo,
                    UpdateUserInfo,
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

        public ContactInfoPresenter UserInfo
        {
            get
            {
                return _userInfo;
            }
            set
            {
                if (Equals(value, _userInfo))
                {
                    return;
                }
                _userInfo = value;
                NotifyOfPropertyChange();
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

        #region Methods

        private void UpdateUserInfo(UserInfo userInfo)
        {
            UserInfo = new ContactInfoPresenter(new UserContactInfo(userInfo));
        }

        #endregion
    }
}
