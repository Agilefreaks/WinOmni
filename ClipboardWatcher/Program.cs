using System;
using System.Windows.Forms;
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
            var kernel = new StandardKernel(new ClipboardWrapperModule(), new MainModule());
            var form = kernel.Get<MainForm>();
            Application.Run(form);
        }
    }
}