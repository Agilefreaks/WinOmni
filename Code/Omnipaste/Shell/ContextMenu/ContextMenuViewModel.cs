namespace Omnipaste.Shell.ContextMenu
{
    using System;
    using System.Deployment.Application;
    using System.Reflection;
    using System.Windows;
    using Caliburn.Micro;
    using CustomizedClickOnce.Common;
    using Ninject;
    using Omni;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework;
    using Omnipaste.Framework.Behaviours;

    public class ContextMenuViewModel : Screen, IContextMenuViewModel
    {
        #region Fields

        private BaloonNotificationInfo _baloonInfo;

        private IClickOnceHelper _clickOnceHelper;

        #endregion
        
        #region Constructors and Destructors

        public ContextMenuViewModel(IOmniService omniService)
        {
            OmniService = omniService;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (ApplicationDeploymentHelper.IsClickOnceApplication)
            {
                var ad = ApplicationDeployment.CurrentDeployment;
                version = ad.CurrentVersion;
            }

            TooltipText = "Omnipaste " + version;
            IconSource = "/Icon.ico";
            AutoStart = ClickOnceHelper.StartupShortcutExists();

            ApplicationWrapper = new ApplicationWrapper();
        }

        #endregion

        #region Public Properties

        public IApplicationWrapper ApplicationWrapper { get; set; }

        public bool AutoStart { get; set; }

        public BaloonNotificationInfo BaloonInfo
        {
            get
            {
                return _baloonInfo;
            }
            set
            {
                _baloonInfo = value;
                NotifyOfPropertyChange(() => BaloonInfo);
            }
        }

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
        public IEventAggregator EventAggregator { get; set; }

        public string IconSource { get; set; }

        public bool IsStopped { get; set; }

        public IOmniService OmniService { get; set; }

        public string TooltipText { get; set; }

        public Visibility Visibility { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Exit()
        {
            ApplicationWrapper.ShutDown();
        }

        public void Show()
        {
            EventAggregator.PublishOnUIThread(new ShowShellMessage());
        }

        public void ShowBaloon(string baloonTitle, string baloonMessage)
        {
            BaloonInfo = new BaloonNotificationInfo { Title = baloonTitle, Message = baloonMessage };
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
            if (IsStopped)
            {
                OmniService.Stop();
            }
            else
            {
                OmniService.Start().Subscribe();
            }
        }

        #endregion
    }
}