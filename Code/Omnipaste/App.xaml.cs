namespace Omnipaste
{
    using System;
    using System.Windows;
    using Omnipaste.Helpers;

    public partial class App
    {
        private static SingleInstanceApp _singleInstance;

        private static readonly string UniqueAppId = Constants.AppName;

        [STAThread]
        public static void Main()
        {
            var application = new App();
            application.InitializeComponent();

            application.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (SingleInstanceApp.InitializeAsFirstInstance(UniqueAppId, out _singleInstance))
            {
                base.OnStartup(e);
            }
            else
            {
                MessageBox.Show(
                    Omnipaste.Properties.Resources.OtherInstanceRunning,
                    Omnipaste.Properties.Resources.Warning,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ApplicationHelper.Instance.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_singleInstance != null) _singleInstance.Dispose();
            base.OnExit(e);
        }
    }
}
