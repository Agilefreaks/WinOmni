namespace Omnipaste.Shell.ContextMenu
{
    using System.Deployment.Application;
    using System.Reflection;
    using System.Windows;
    using Caliburn.Micro;
    using CustomizedClickOnce.Common;
    using Ninject;
    using Omni;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework;

    public class ContextMenuViewModel : Screen, IContextMenuViewModel
    {
        private IClickOnceHelper _clickOnceHelper;

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

        public bool AutoStart { get; set; }

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

        public string IconSource { get; set; }

        public bool IsStopped { get; set; }

        public string TooltipText { get; set; }

        public IApplicationWrapper ApplicationWrapper { get; set; }

        public Visibility Visibility { get; set; }

        public IOmniService OmniService { get; set; }

        [Inject]
        public IEventAggregator EventAggregator { get; set; }

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

        public async void ToggleSync()
        {
            if (IsStopped)
            {
                OmniService.Stop();
            }
            else
            {
                await OmniService.Start();
            }
        }

        #endregion
    }
}