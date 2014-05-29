using System;
using System.IO;

namespace CustomizedClickOnce.Install
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;

    static class Program
    {
        private static Mutex _instanceMutex;

#if STAGING
        private static string applicationURL = "http://cdn.omnipasteapp.com/staging/win/Omnipaste-staging.application?token={0}";
#endif

#if RELEASE
        private static string applicationURL = "http://cdn.omnipasteapp.com/production/win/Omnipaste.application?token={0}";
#endif

#if DEBUG
        private static string applicationURL = "http://cdn.omnipasteapp.com/staging/win/Omnipaste-staging.application?token={0}";
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {           
            bool createdNew;
            _instanceMutex = new Mutex(
                true, @"Local\" + Assembly.GetExecutingAssembly().GetType().GUID, out createdNew);
            if (createdNew)
            {
                StartIEProcessWithActivationToken();
            }
            else
            {
                _instanceMutex = null;
            }
        }

        private static void StartIEProcessWithActivationToken()
        {
            var activationToken = GetActivationTokenFromFileName(Process.GetCurrentProcess().MainModule.FileName);
            if (!string.IsNullOrEmpty(activationToken))
            {
                Process.Start("iexplore", string.Format(applicationURL, activationToken));
            }
        }

        private static string GetActivationTokenFromFileName(string fileName)
        {
            var result = string.Empty;

            try
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                result = fileNameWithoutExtension.Substring(14, 6);
            }
            catch (ArgumentOutOfRangeException){ }

            return result;
        }
    }
}
