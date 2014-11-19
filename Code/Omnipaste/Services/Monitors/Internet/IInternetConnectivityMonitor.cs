﻿namespace Omnipaste.Services.Monitors.Internet
{
    using System;
    using Ninject;

    public interface IInternetConnectivityMonitor : IStartable
    {
        IObservable<InternetConnectivityStatusEnum> ConnectivityChangedObservable { get; }
    }
}