﻿using System;
using System.Windows.Forms;
using ClipboardWrapper;
using Ninject;
using Omniclipboard;

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
            var kernel = new StandardKernel(new MainModule(), new ClipboardWrapperModule(), new CloudClipboardModule());
            var form = kernel.Get<MainForm>();
            Application.Run(form);
        }
    }
}