namespace Omnipaste.Services
{
    using System;
    using System.Windows;
    using System.Windows.Threading;
    using OmniCommon;

    public class GlobalExceptionLogger
    {
        public static void Hook()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            SimpleLogger.Log("Unhandled DOMAIN Exception: " + args.ExceptionObject);
        }

        private static void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            SimpleLogger.Log("Unhandled DISPATCHER Exception: " + args.Exception);
        }
    }
}
