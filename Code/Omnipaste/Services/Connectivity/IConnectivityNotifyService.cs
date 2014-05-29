using System;

namespace Omnipaste.Services.Connectivity
{
    public interface IConnectivityNotifyService
    {
        event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;
        
        void Start();

        void Stop();
    }
}