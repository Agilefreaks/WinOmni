namespace Omnipaste.Helpers
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using Caliburn.Micro;
    using OmniCommon.Interfaces;

    public class ExternalProcessHelper
    {
        #region Public Methods and Operators

        public static void ShowVideoTutorial()
        {
            var configurationService = IoC.Get<IConfigurationService>();
            var videoTutorialAddress = new Uri(configurationService.WebBaseUrl + "/#video").ToString();
            Start(videoTutorialAddress);
        }

        public static void Start(string process)
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

        #endregion
    }
}