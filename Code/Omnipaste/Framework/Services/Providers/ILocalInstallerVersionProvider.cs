namespace Omnipaste.Framework.Services.Providers
{
    using System;

    public interface ILocalInstallerVersionProvider
    {
        Version GetVersion(string installerLocation);
    }
}