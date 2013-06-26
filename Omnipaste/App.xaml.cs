namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using System.Deployment.Application;
    using System.Windows;
    using CustomizedClickOnce.Common;

    public partial class App : ISingleInstanceApp
    {
        private const string Unique = "Omnipaste";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                PerformFirstRunTasks();

                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
            else
            {
                MessageBox.Show("Another instance is already running. Check for the tray icon.", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }

        private static void PerformFirstRunTasks()
        {
            if (!ApplicationDeployment.IsNetworkDeployed || !ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                return;
            }

            var clickOnceHelper = new ClickOnceHelper(ApplicationInfoFactory.Create());
            clickOnceHelper.UpdateUninstallParameters();
            clickOnceHelper.AddShortcutToStartup();
        }
    }
}
