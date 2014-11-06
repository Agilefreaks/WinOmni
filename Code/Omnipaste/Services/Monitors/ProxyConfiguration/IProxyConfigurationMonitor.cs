namespace Omnipaste.Services.Monitors.ProxyConfiguration
{
    using System;
    using Ninject;
    using OmniCommon;
    using OmniCommon.Interfaces;

    public interface IProxyConfigurationMonitor : IProxyConfigurationObserver, IStartable
    {
        IObservable<ProxyConfiguration> ProxyConfigurationObservable { get; }
    }
}