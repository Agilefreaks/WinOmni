namespace Omnipaste
{
    using System;
    using System.Deployment.Application;
    using System.Windows;
    using CustomizedClickOnce.Common;

    public partial class App
    {
        private const string Unique = "Omnipaste";

        [STAThread]
        public static void Main()
        {
            SingleInstanceApp singleInstance;
            if (SingleInstanceApp.InitializeAsFirstInstance(Unique, out singleInstance))
            {
                using (singleInstance)
                {
                    PerformFirstRunTasks();

                    var application = new App();
                    application.InitializeComponent();

                    application.Run();
                }
            }
            else
            {
                MessageBox.Show("Another instance is already running. Check for the tray icon.", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        
        private static void PerformFirstRunTasks()
        {
            if (!ApplicationDeploymentHelper.IsClickOnceApplication || !ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                return;
            }

            var clickOnceHelper = new ClickOnceHelper(ApplicationInfoFactory.Create());
            clickOnceHelper.UpdateUninstallParameters();
            clickOnceHelper.AddShortcutToStartup();
        }
    }
}
