namespace Omnipaste.Services.Connectivity
{
    using System;
    using Ninject;

    public interface IConnectivityNotifyService : IStartable
    {
        bool CurrentlyConnected { get; }

        IObservable<bool> ConnectivityChangedObservable { get; }
    }
}