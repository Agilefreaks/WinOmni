using System;
using System.Windows.Forms;
using Ninject;
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
            var kernel = new StandardKernel(new MainModule(), new ClipboardWrapperModule(), new OmniclipboardModule());
            var form = kernel.Get<MainForm>();
            Application.Run(form);
        }
    }
}