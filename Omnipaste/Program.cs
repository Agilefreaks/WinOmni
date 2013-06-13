﻿namespace Omnipaste
{
    using System;
    using System.Deployment.Application;
    using System.Windows.Forms;
    using CustomizedClickOnce.Common;
    using Ninject;
    using OmniCommon;
    using Omnipaste.Properties;
    using PubNubClipboard;
    using WindowsClipboard;

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool createdNewMutex;
            using (new System.Threading.Mutex(true, "Omnipaste", out createdNewMutex))
            {
                if (createdNewMutex)
                {
                    ConfigureAndRun();
                }
                else
                {
                    MessageBox.Show(Resources.InstanceAlreadyRunning);
                }
            }
        }

        private static void ConfigureAndRun()
        {
            SetupUninstaller();
            var mainModule = new MainModule();
            var kernel = new StandardKernel(
                mainModule,
                new CommonModule(),
                new WindowsClipboardModule(),
                new PubNubClipboardModule());

            mainModule.PerfornStartupTasks();
            var form = kernel.Get<MainForm>();
            Application.Run(form);
        }

        private static void SetupUninstaller()
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