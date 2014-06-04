using System;
using Ninject;

namespace Omnipaste.Services.Connectivity
{
    public interface IConnectivityNotifyService : IStartable
    {
        event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

        void Stop();
    }
}