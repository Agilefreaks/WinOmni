namespace Omnipaste.Shell.ContextMenu
{
    using System;
    using System.Reactive.Linq;
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
        #region Fields

        private readonly IDisposable _statusChangedObserver;

        private BalloonNotificationInfo _balloonInfo;

        private string _iconSource;

        #endregion

        #region Constructors and Destructors

        public ContextMenuViewModel(IOmniService omniService)
        {
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

        public string TooltipText
        {
            get
            {
                return string.Format("{0} {1}", Constants.AppName, ConfigurationService.Version);
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

        #endregion
    }
}