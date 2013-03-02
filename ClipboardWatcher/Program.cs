using System;
using System.Windows.Forms;
using ClipboardWatcher.Core;
using ClipboardWrapper;
using Ninject;

namespace ClipboardWatcher
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
            var kernel = new StandardKernel(new ClipboardWrapperModule(), new ClipboardWatcherCoreModule(),
                                            new MainModule());
            var form = kernel.Get<MainForm>();
            Application.Run(form);
        }
    }
}