namespace Omnipaste.Services.Providers
{
    using System;

    public interface IApplicationVersionProvider
    {
        Version GetVersion();
    }
}