namespace Omnipaste.ContextMenu
{
    using System.Windows;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework;

    public class ContextMenuViewModel : Screen, IContextMenuViewModel
    {
        private string _iconSource;

        private string _tooltipText;

        private Visibility _visibility;

        private bool _autoStart;

        private string _channel;

        private bool _isSyncing;

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
                return _tooltipText;
            }

            set
            {
                _tooltipText = value;
                NotifyOfPropertyChange(() => TooltipText);
            }
        }

        public IApplicationWrapper ApplicationWrapper { get; set; }

        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }

            set
            {
                _visibility = value;
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        public bool AutoStart
        {
            get
            {
                return _autoStart;
            }

            set
            {
                _autoStart = value;
                NotifyOfPropertyChange(() => AutoStart);
            }
        }

        public string Channel
        {
            get
            {
                return _channel;
            }

            set
            {
                _channel = value;
                NotifyOfPropertyChange(() => Channel);
            }
        }

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IOmniService OmniService { get; set; }

        public ContextMenuViewModel()
        {
            IconSource = "/Icon.ico";
            TooltipText = "Omnipaste";

            ApplicationWrapper = new ApplicationWrapper();
        }

        public void ToggleAutoStart()
        {
        }

        public void ToggleSync()
        {
        }

        public void Start()
        {
            Channel = ConfigurationService.CommunicationSettings.Channel;

            AutoStart = true;

            OmniService.Start();
        }

        public void Exit()
        {
            Visibility = Visibility.Collapsed;
            ApplicationWrapper.ShutDown();
        }
    }
}