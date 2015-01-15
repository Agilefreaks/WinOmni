namespace Omnipaste.Services.Providers
{
    using System;
    using System.Linq;

    public class RemoteInstallerVersionProvider : IRemoteInstallerVersionProvider
    {
        private static IRemoteInstallerVersionProvider _instance;

        public static IRemoteInstallerVersionProvider Instance
        {
            get
            {
                return _instance ?? (_instance = new RemoteInstallerVersionProvider());
            }
            set
            {
                _instance = value;
            }
        }

        Version IRemoteInstallerVersionProvider.GetVersion(IUpdateManager updateManager, string installerName)
        {
            return
                updateManager.GetUpdatedFiles()
                    .Where(fileUpdateTask => fileUpdateTask.LocalPath == installerName)
                    .Select(fileUpdateTask => fileUpdateTask.Version)
                    .FirstOrDefault();
        }

        public static Version GetVersion(IUpdateManager updateManager, string installerName)
        {
            return Instance.GetVersion(updateManager, installerName);
        }
    }
}