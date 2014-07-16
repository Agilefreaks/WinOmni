using System;
using System.IO;

namespace CustomizedClickOnce.Install
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;

    static class Installer
    {
        private static Mutex _instanceMutex;

#if STAGING
        private const string ApplicationUrl = "http://download.omnipasteapp.com/staging/Omnipaste-staging.application?token={0}";
#endif

#if RELEASE
        private const string ApplicationUrl = "http://download.omnipasteapp.com/production/Omnipaste.application?token={0}";
#endif

#if DEBUG
        private const string ApplicationUrl = "http://localhost/Omnipaste-staging.application?token={0}";

#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static int Main()
        {           
            bool createdNew;
            _instanceMutex = new Mutex(
                true, @"Local\" + Assembly.GetExecutingAssembly().GetType().GUID, out createdNew);
            if (createdNew)
            {
                StartIeProcessWithActivationToken();
                ReleaseMutex();
            }
            else
            {
                _instanceMutex = null;
            }

            return 0;
        }

        private static void ReleaseMutex()
        {
            if (_instanceMutex == null)
            {
                return;
            }

            _instanceMutex.ReleaseMutex();
            _instanceMutex.Close();
            _instanceMutex = null;
        }

        private static void StartIeProcessWithActivationToken()
        {
            var activationToken = GetActivationTokenFromFileName(Process.GetCurrentProcess().MainModule.FileName);
            if (!string.IsNullOrEmpty(activationToken))
            {
                Process.Start("iexplore", string.Format(ApplicationUrl, activationToken));
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
