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
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework.Behaviours;

    public class ContextMenuViewModel : Screen, IContextMenuViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        #region Fields

        private readonly IDisposable _statusChangedObserver;

        private BalloonNotificationInfo _balloonInfo;

        private string _iconSource;

        #endregion

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
            System.Action closeApp = () => ApplicationService.ShutDown();
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

        public void Handle(ApplicationClosingMessage message)
        {
            Visibility = Visibility.Hidden;
        }
    }
}