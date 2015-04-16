namespace Omnipaste.Framework.Services.Providers
{
    using System;

    public interface IRemoteInstallerVersionProvider
    {
        Version GetVersion(IUpdateManager updateManager, string installerName);
    }
}