namespace Omnipaste.Framework
{
    using System;
    using System.Deployment.Application;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;
    using BugFreak;
    using Microsoft.Win32;
    using OmniCommon.Interfaces;

    public class ApplicationService : IApplicationService
    {
        #region Constants

        private const string AutoStartArguments = "-minimized";

        #endregion

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

        public Version Version
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;

                if (ApplicationDeploymentHelper.IsClickOnceApplication)
                {
                    ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                    version = ad.CurrentVersion;
                }

                return version;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ShutDown()
        {
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
                ReportingService.Instance.BeginReport(exception);
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
                ReportingService.Instance.BeginReport(exception);
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
                ReportingService.Instance.BeginReport(exception);
            }

            return result;
        }

        #endregion
    }
}