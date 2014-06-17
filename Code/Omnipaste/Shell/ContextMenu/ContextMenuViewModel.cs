namespace Omnipaste.Shell.ContextMenu
{
    using System.Deployment.Application;
    using System.Reflection;
    using System.Windows;
    using Caliburn.Micro;
    using Omni;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework;

    public class ContextMenuViewModel : Screen, IContextMenuViewModel
    {
        #region Constructors and Destructors

        public ContextMenuViewModel(IConfigurationService configurationService, IOmniService omniService)
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
            AutoStart = configurationService.AutoStart;

            ApplicationWrapper = new ApplicationWrapper();
        }

        #endregion

        #region Public Properties

        public bool AutoStart { get; set; }

        public string IconSource { get; set; }

        public bool IsSyncing { get; set; }

        public string TooltipText { get; set; }

        public IApplicationWrapper ApplicationWrapper { get; set; }

        public Visibility Visibility { get; set; }

        public IShellViewModel ShellViewModel { get; set; }

        public IOmniService OmniService { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Exit()
        {
            ApplicationWrapper.ShutDown();
        }

        public void Show()
        {
            ShellViewModel.Show();
        }

        public void ToggleAutoStart()
        {

        }

        public void ToggleSync()
        {
            if (IsSyncing)
            {
                OmniService.Start();
            }
            else
            {
                OmniService.Stop();
            }
        }

        #endregion
    }
}