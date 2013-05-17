using System;
using System.Windows.Forms;
using Ninject;
using OmniCommon;
using PubNubClipboard;
using WindowsClipboard;

namespace Omnipaste
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool ok;
            var m = new System.Threading.Mutex(true, "Omnipaste", out ok);
            if (!ok)
            {
                MessageBox.Show("Another instance is already running.");
                return;
            }

            ConfigureAndRun();
        }

        private static void ConfigureAndRun()
        {
            var kernel = new StandardKernel(new MainModule(), new CommonModule(), new WindowsClipboardModule(),
                                            new PubNubClipboardModule());
            var form = kernel.Get<MainForm>();
            Application.Run(form);
        }
    }
}