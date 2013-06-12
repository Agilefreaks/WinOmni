namespace Omnipaste
{
    using System;
    using System.Windows.Forms;
    using Ninject;
    using OmniCommon;
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
            using (var mutex = new System.Threading.Mutex(true, "Omnipaste", out createdNewMutex))
            {
                if (!createdNewMutex)
                {
                    MessageBox.Show("Another instance is already running.");
                }
                else
                {
                    ConfigureAndRun();
                }
            }
        }

        private static void ConfigureAndRun()
        {
            var mainModule = new MainModule();
            var kernel = new StandardKernel(
                mainModule, new CommonModule(), new WindowsClipboardModule(), new PubNubClipboardModule());
            mainModule.PerfornStartupTasks();
            var form = kernel.Get<MainForm>();
            Application.Run(form);
        }
    }
}