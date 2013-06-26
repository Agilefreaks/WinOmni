﻿namespace Omnipaste.ContextMenu
{
    using System.Deployment.Application;
    using System.Reflection;
    using System.Windows;
    using Caliburn.Micro;
    using CustomizedClickOnce.Common;
    using Ninject;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework;

    public class ContextMenuViewModel : Screen, IContextMenuViewModel
    {
        private string _iconSource;

        private string _tooltipText;

        private IClickOnceHelper _clickOnceHelper;

        private Visibility _visibility;

        private bool _autoStart;

        private string _channel;

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

        public bool IsNotSyncing { get; set; }

        public IClickOnceHelper ClickOnceHelper
        {
            get
            {
                return _clickOnceHelper ?? (_clickOnceHelper = new ClickOnceHelper(ApplicationInfoFactory.Create()));
            }

            set
            {
                _clickOnceHelper = value;
            }
        }

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IOmniService OmniService { get; set; }

        public ContextMenuViewModel()
        {
            IconSource = "/Icon.ico";

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var ad = ApplicationDeployment.CurrentDeployment;
                version = ad.CurrentVersion;
            }

            TooltipText = "Omnipaste " + version;

            ApplicationWrapper = new ApplicationWrapper();
        }

        public void ToggleAutoStart()
        {
            if (AutoStart)
            {
                ClickOnceHelper.AddShortcutToStartup();
            }
            else
            {
                ClickOnceHelper.RemoveShortcutFromStartup();
            }
        }

        public void ToggleSync()
        {
            if (IsNotSyncing)
            {
                OmniService.Stop();
            }
            else
            {
                OmniService.Start();
            }
        }

        public void Start()
        {
            Channel = ConfigurationService.CommunicationSettings.Channel;

            AutoStart = ClickOnceHelper.StartupShortcutExists();

            OmniService.Start();
        }

        public void Exit()
        {
            Visibility = Visibility.Collapsed;
            ApplicationWrapper.ShutDown();
        }
    }
}