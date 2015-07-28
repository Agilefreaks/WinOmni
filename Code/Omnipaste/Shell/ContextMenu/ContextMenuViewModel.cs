namespace Omnipaste.Shell.ContextMenu
{
    using System;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using Ninject;
    using Omni;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework.Behaviours;
    using Omnipaste.Framework.EventAggregatorMessages;
    using Action = System.Action;

    public class ContextMenuViewModel : Screen, IContextMenuViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        #region Constructors and Destructors

        public ContextMenuViewModel(IOmniService omniService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            IconSource = "/Disconnected.ico";
            _statusChangedObserver =
                omniService.StatusChangedObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(
                        status =>
                        IconSource = status == OmniServiceStatusEnum.Started ? "/Connected.ico" : "/Disconnected.ico");
        }

        #endregion

        #region IContextMenuViewModel Members

        public void Handle(ApplicationClosingMessage message)
        {
            Visibility = Visibility.Hidden;
        }

        #endregion

        #region Fields

        private readonly IDisposable _statusChangedObserver;

        private BalloonNotificationInfo _balloonInfo;

        private string _iconSource;

        private bool _pause;

        #endregion

        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

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

        public bool Pause
        {
            get
            {
                return ConfigurationService.PauseNotifications;
            }
            set
            {
                ConfigurationService.PauseNotifications = value;
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
                if (_balloonInfo != value)
                {
                    _balloonInfo = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public string IconSource
        {
            get
            {
                return _iconSource;
            }
            set
            {
                if (_iconSource != value)
                {
                    _iconSource = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public string TooltipText
        {
            get
            {
                return ConfigurationService.AppNameAndVersion;
            }
        }

        public Visibility Visibility { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _statusChangedObserver.Dispose();
        }

        public void Exit()
        {
            Action closeApp = () => ApplicationService.ShutDown();
            var dispatcher = DispatcherProvider.Current;
            Task.Delay(TimeSpan.FromMilliseconds(500)).ContinueWith(_ => dispatcher.Dispatch(closeApp));
        }

        public void Show()
        {
            _eventAggregator.PublishOnUIThread(new ShowShellMessage());
        }

        public void ShowBalloon(string balloonTitle, string balloonMessage)
        {
            if (Visibility == Visibility.Visible)
            {
                BalloonInfo = new BalloonNotificationInfo { Title = balloonTitle, Message = balloonMessage };
            }
        }

        #endregion
    }
}