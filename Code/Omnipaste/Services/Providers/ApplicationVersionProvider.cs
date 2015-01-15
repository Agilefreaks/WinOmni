namespace Omnipaste.Services.Providers
{
    using System;
    using System.Reflection;

    public class ApplicationVersionProvider : IApplicationVersionProvider
    {
        private static IApplicationVersionProvider _instance;

        public static IApplicationVersionProvider Instance
        {
            get
            {
                return _instance ?? (_instance = new ApplicationVersionProvider());
            }
            set
            {
                _instance = value;
            }
        }

        Version IApplicationVersionProvider.GetVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }

        public static Version GetVersion()
        {
            return Instance.GetVersion();
        }
    }
}