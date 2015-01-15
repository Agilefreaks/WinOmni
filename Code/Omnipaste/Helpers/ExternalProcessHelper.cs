namespace Omnipaste.Helpers
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using Caliburn.Micro;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;

    public class ExternalProcessHelper : IProcessHelper
    {
        private static IProcessHelper _instance;

        public static IProcessHelper Instance
        {
            get
            {
                return _instance ?? (_instance = new ExternalProcessHelper());
            }
            set
            {
                _instance = value;
            }
        }

        void IProcessHelper.Start(string process)
        {
            try
            {
                Process.Start(process);
            }
            catch (Win32Exception)
            {
                // Looks like there is no way for us to act on this
            }
        }

        void IProcessHelper.Start(ProcessStartInfo processInfo)
        {
            try
            {
                Process.Start(processInfo);
            }
            catch (Win32Exception)
            {
            }
            catch (Exception exception)
            {
                ExceptionReporter.Instance.Report(exception);
            }
        }

        public static void ShowVideoTutorial()
        {
            var configurationService = IoC.Get<IConfigurationService>();
            var videoTutorialAddress = new Uri(configurationService.WebBaseUrl + "/#video").ToString();
            Start(videoTutorialAddress);
        }

        public static void Start(string process)
        {
            Instance.Start(process);
        }

        public static void Start(ProcessStartInfo processStartInfo)
        {
            Instance.Start(processStartInfo);
        }
    }
}