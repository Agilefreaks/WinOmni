namespace Omnipaste.Services.Connectivity
{
    using System;
    using Ninject;

    public interface IConnectivityNotifyService : IStartable
    {
        event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

        bool PreviouslyConnected { get; set; }

        bool CurrentlyConnected { get; }
    }
}