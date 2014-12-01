namespace Omnipaste.Services.Monitors.ProxyConfiguration
{
    using System;
    using Ninject;
    using OmniCommon;

    public interface IProxyConfigurationMonitor : IStartable
    {
        IObservable<ProxyConfiguration> ProxyConfigurationObservable { get; }
    }
}