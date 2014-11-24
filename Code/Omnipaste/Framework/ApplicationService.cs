namespace Omnipaste.Framework
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;
    using Caliburn.Micro;
    using Microsoft.Win32;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;

    public class ApplicationService : IApplicationService
    {
        private readonly IEventAggregator _eventAggregator;

        #region Constants

        private const string AutoStartArguments = "-minimized";

        #endregion

        public ApplicationService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #region Public Properties

        public bool AutoStart
        {
            get
            {
                return IsAutoStartEnabled();
            }
            set
            {
                if (value)
                {
                    EnableAutoStart();
                }
                else
                {
                    DisableAutoStart();
                }
            }
        }

        public Dispatcher Dispatcher
        {
            get
            {
                return Application.Current.Dispatcher;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ShutDown()
        {
            _eventAggregator.PublishOnUIThread(new ApplicationClosingMessage());

            Application.Current.Shutdown();
        }

        #endregion

        #region Methods

        private static string GetExecutableLocation()
        {
            return Assembly.GetEntryAssembly().Location;
        }

        private static RegistryKey GetStartupRegistryKey()
        {
            return Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        }

        private void DisableAutoStart()
        {
            try
            {
                GetStartupRegistryKey().DeleteValue(Constants.AppName, false);
            }
            catch (Exception exception)
            {
                ExceptionReporter.Instance.Report(exception);
            }
        }

        private void EnableAutoStart()
        {
            try
            {
                var subKey = GetStartupRegistryKey();
                subKey.SetValue(
                    Constants.AppName,
                    string.Format("\"{0}\" {1}", GetExecutableLocation(), AutoStartArguments),
                    RegistryValueKind.String);
            }
            catch (Exception exception)
            {
                ExceptionReporter.Instance.Report(exception);
            }
        }

        private bool IsAutoStartEnabled()
        {
            var result = false;
            try
            {
                result = GetStartupRegistryKey().GetValueNames().Contains(Constants.AppName);
            }
            catch (Exception exception)
            {
                ExceptionReporter.Instance.Report(exception);
            }

            return result;
        }

        #endregion
    }
}