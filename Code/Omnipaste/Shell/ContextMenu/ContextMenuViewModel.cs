namespace Omnipaste.Shell.ContextMenu
{
    using System.Windows;
    using Caliburn.Micro;
    using Ninject;
    using Omni;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Framework.Behaviours;
    using OmniSync;

    public class ContextMenuViewModel : Screen, IContextMenuViewModel
    {
        #region Fields

        private readonly IOmniService _omniService;

        private BalloonNotificationInfo _balloonInfo;

        private string _iconSource;

        #endregion

        #region Constructors and Destructors

        public ContextMenuViewModel(IOmniService omniService)
        {
            _omniService = omniService;
            _omniService.StatusChangedObservable.SubscribeAndHandleErrors(
                status => { IconSource = status == ServiceStatusEnum.Started ? "/Connected.ico" : "/Disconnected.ico"; });

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

        public bool IsStopped { get; set; }

        public string TooltipText
        {
            get
            {
                return "Omnipaste " + ApplicationService.Version;
            }
        }

        public Visibility Visibility { get; set; }

        #endregion

        #region Public Methods and Operators

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
            if (IsStopped)
            {
                _omniService.StartWithDefaultObserver();
            }
            else
            {
                _omniService.StopWithDefaultObserver();
            }
        }

        #endregion
    }
}