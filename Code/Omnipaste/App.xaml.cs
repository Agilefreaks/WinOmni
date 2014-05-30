namespace Omnipaste
{
    using System;
    using System.Collections.Generic;
    using System.Deployment.Application;
    using System.Windows;
    using System.Configuration;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            BugFreak.BugFreak.Hook(
                ConfigurationManager.AppSettings["BugFreakApiKey"],
                ConfigurationManager.AppSettings["BugFreakToken"],
                this);
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
