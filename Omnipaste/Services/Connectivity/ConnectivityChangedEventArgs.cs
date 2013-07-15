using System;

namespace Omnipaste.Services.Connectivity
{
    public class ConnectivityChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; private set; }

        public ConnectivityChangedEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}
