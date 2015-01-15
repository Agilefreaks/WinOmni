namespace Omnipaste.Services.Providers
{
    using System;

    public interface ILocalInstallerVersionProvider
    {
        Version GetVersion(string installerLocation);
    }
}