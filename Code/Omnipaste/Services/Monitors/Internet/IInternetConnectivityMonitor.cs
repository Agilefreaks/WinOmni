﻿namespace Omnipaste.Services.Monitors.Internet
{
    using System;
    using Ninject;

    public interface IInternetConnectivityMonitor : IStartable
    {
        bool CurrentlyConnected { get; }

        IObservable<InternetConnectivityStatusEnum> ConnectivityChangedObservable { get; }
    }
}