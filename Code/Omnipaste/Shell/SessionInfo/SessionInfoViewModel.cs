namespace Omnipaste.Shell.SessionInfo
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Text;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Ninject;
    using Omni;
    using OmniCommon;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Profile;
    using Omnipaste.Properties;
    using OmniUI.Resources;
    using OmniUI.Workspaces;

    public class SessionInfoViewModel : Screen, ISessionInfoViewModel
    {
        #region Constructors and Destructors

        public SessionInfoViewModel(IOmniService omniService, IConfigurationService configurationService)
        {
            _statusTexts = new Dictionary<ConnectionStateEnum, string>
                               {
                                   {
                                       ConnectionStateEnum.Connected,
                                       Resources.Connected
                                   },
                                   {
                                       ConnectionStateEnum.Disconnected,
                                       Resources.Disconnected
                                   }
                               };
            _icons = new Dictionary<ConnectionStateEnum, string>
                         {
                             {
                                 ConnectionStateEnum.Connected,
                                 IconNames.Connected
                             },
                             {
                                 ConnectionStateEnum.Disconnected,
                                 IconNames.Disconnected
                             }
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

        #region Methods

        private void UpdateUserInfo(UserInfo userInfo)
        {
            User = new ContactModel(new UserEntity(userInfo));
        }

        #endregion

        #region Fields

        private readonly Dictionary<ConnectionStateEnum, string> _icons;

        private readonly Dictionary<ConnectionStateEnum, string> _statusTexts;

        private readonly IDisposable _statusObserver;

        private readonly IDisposable _userObserver;

        private ConnectionStateEnum _state;

        private ContactModel _user;

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

        public ContactModel User
        {
            get
            {
                return _user;
            }
            set
            {
                if (Equals(value, _user))
                {
                    return;
                }
                _user = value;
                NotifyOfPropertyChange();
            }
        }

        [Inject]
        public IWorkspaceConductor WorkspaceConductor { get; set; }

        [Inject]
        public IProfileWorkspace ProfileWorkspace { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _statusObserver.Dispose();
            _userObserver.Dispose();
        }

        public void ShowUserProfile()
        {
            WorkspaceConductor.ActivateItem(ProfileWorkspace);
        }

        #endregion
    }
}