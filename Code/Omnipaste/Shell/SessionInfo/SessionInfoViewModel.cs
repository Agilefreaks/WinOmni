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
    using Omnipaste.Presenters;
    using Omnipaste.Properties;
    using OmniUI.Resources;

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
                                { ConnectionStateEnum.Connected, Resources.Connected },
                                { ConnectionStateEnum.Disconnected, Resources.Disconnected }
                            };
            _icons = new Dictionary<ConnectionStateEnum, string>
                         {
                             { ConnectionStateEnum.Connected, IconNames.Connected },
                             { ConnectionStateEnum.Disconnected, IconNames.Disconnected }
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
                if (_state != value)
                {
                    _state = value;
                    NotifyOfPropertyChange();
                    NotifyOfPropertyChange(() => StatusText);
                    NotifyOfPropertyChange(() => Icon);
                }
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
            UserInfo = new ContactInfoPresenter(new UserContactEntity(userInfo));
        }

        #endregion
    }
}
