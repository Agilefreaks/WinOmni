﻿namespace Omnipaste
{
    using System;
    using System.Windows;

    public partial class App
    {
        private static SingleInstanceApp _singleInstance;

        private const string Unique = "Omnipaste";

        [STAThread]
        public static void Main()
        {
            var application = new App();
            application.InitializeComponent();

            application.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (SingleInstanceApp.InitializeAsFirstInstance(Unique, out _singleInstance))
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
                Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_singleInstance != null) _singleInstance.Dispose();
            base.OnExit(e);
        }
    }
}
