namespace Omnipaste.Shell.ContextMenu
{
    using System;
    using System.Windows;
    using Caliburn.Micro;
    using Ninject;
    using Omni;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework.Behaviours;
    using Omnipaste.Services.Monitors.User;

    public class ContextMenuViewModel : Screen, IContextMenuViewModel
    {
        #region Fields

        private readonly IUserMonitor _userMonitor;

        private BalloonNotificationInfo _balloonInfo;

        private string _iconSource;

        private bool _isStopped;

        private bool _canToggleSync;

        private IDisposable _statusChangedObserver;

        private IDisposable _inTransitionChangedObserver;

        #endregion

        #region Constructors and Destructors

        public ContextMenuViewModel(IOmniService omniService, IUserMonitor userMonitor)
        {
            _userMonitor = userMonitor;
            _statusChangedObserver = omniService.StatusChangedObservable.SubscribeAndHandleErrors(
                status =>
                {
                    IconSource = status == OmniServiceStatusEnum.Started ? "/Connected.ico" : "/Disconnected.ico";
                    IsStopped = status == OmniServiceStatusEnum.Stopped;
                });
            _inTransitionChangedObserver = omniService.InTransitionObservable.SubscribeAndHandleErrors(
                isInTransition => CanToggleSync = !isInTransition);
            IconSource = "/Disconnected.ico";
        }

        #endregion

        #region Public Properties

        [Inject]
        public IApplicationService ApplicationService { get; set; }

        public bool AutoStart
        {
            get
            {
                return ApplicationService.AutoStart;
            }
            set
            {
                if (value.Equals(ApplicationService.AutoStart))
                {
                    return;
                }
                ApplicationService.AutoStart = value;
                NotifyOfPropertyChange();
            }
        }

        public BalloonNotificationInfo BalloonInfo
        {
            get
            {
                return _balloonInfo;
            }
            set
            {
                _balloonInfo = value;
                NotifyOfPropertyChange(() => BalloonInfo);
            }
        }

        [Inject]
        public IEventAggregator EventAggregator { get; set; }

        public string IconSource
        {
            get
            {
                return _iconSource;
            }
            set
            {
                _iconSource = value;
                NotifyOfPropertyChange(() => IconSource);
            }
        }

        public bool IsStopped
        {
            get
            {
                return _isStopped;
            }
            set
            {
                _isStopped = value;
                NotifyOfPropertyChange();
            }
        }

        public string TooltipText
        {
            get
            {
                return string.Format("{0} {1}", Constants.AppName, ApplicationService.Version);
            }
        }

        public Visibility Visibility { get; set; }

        public bool CanToggleSync
        {
            get
            {
                return _canToggleSync;
            }
            set
            {
                if (value.Equals(_canToggleSync))
                {
                    return;
                }
                _canToggleSync = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _statusChangedObserver.Dispose();
            _inTransitionChangedObserver.Dispose();
        }

        public void Exit()
        {
            ApplicationService.ShutDown();
        }

        public void Show()
        {
            EventAggregator.PublishOnUIThread(new ShowShellMessage());
        }

        public void ShowBalloon(string balloonTitle, string balloonMessage)
        {
            BalloonInfo = new BalloonNotificationInfo { Title = balloonTitle, Message = balloonMessage };
        }

        public void ToggleSync()
        {
            _userMonitor.SendEvent(IsStopped ? UserEventTypeEnum.Disconnect : UserEventTypeEnum.Connect);
        }

        #endregion
    }
}