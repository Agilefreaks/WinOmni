using System;

namespace CustomizedClickOnce.Install
{
    using System.Configuration;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;

    static class Program
    {
        private static Mutex _instanceMutex;

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
            var omnipasteCDN = ConfigurationManager.AppSettings["OmnipasteGenericCDN"];
            if (!string.IsNullOrEmpty(omnipasteCDN) && !string.IsNullOrEmpty(activationToken))
            {
                Process.Start("iexplore", string.Concat(omnipasteCDN, activationToken));
            }
        }

        private static string GetActivationTokenFromFileName(string fileName)
        {
            var result = string.Empty;
            const string GuidPattern = @"([a-z0-9]*[-]){4}[a-z0-9]*";

            var mc = Regex.Matches(fileName, GuidPattern);
            if (mc.Count != 0)
            {
                result = mc[0].Value;
            }

            return result;
        }
    }
}
