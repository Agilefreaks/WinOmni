namespace Omnipaste.Framework.Services.Providers
{
    using System;
    using System.IO;
    using Microsoft.Deployment.WindowsInstaller;

    public class LocalInstallerVersionProvider : ILocalInstallerVersionProvider
    {
        private static ILocalInstallerVersionProvider _instance;

        public static ILocalInstallerVersionProvider Instance
        {
            get
            {
                return _instance ?? (_instance = new LocalInstallerVersionProvider());
            }
            set
            {
                _instance = value;
            }
        }

        Version ILocalInstallerVersionProvider.GetVersion(string installerLocation)
        {
            if (!File.Exists(installerLocation))
            {
                return null;
            }

            string versionString;
            using (var database = new Database(installerLocation))
            {
                versionString =
                    database.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = '{0}'", "ProductVersion")
                    as string;
            }

            Version version;
            Version.TryParse(versionString, out version);

            return version;
        }

        public static Version GetVersion(string installerLocation)
        {
            return Instance.GetVersion(installerLocation);
        }
    }
}