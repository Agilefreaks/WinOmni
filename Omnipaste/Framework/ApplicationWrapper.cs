﻿namespace Omnipaste.Framework
{
    using System.Windows;
    using System.Windows.Threading;

    public class ApplicationWrapper : IApplicationWrapper
    {
        public Dispatcher Dispatcher
        {
            get
            {
                return Application.Current.Dispatcher;
            }
        }

        public void ShutDown()
        {
            Dispatcher.InvokeShutdown();
        }
    }
}