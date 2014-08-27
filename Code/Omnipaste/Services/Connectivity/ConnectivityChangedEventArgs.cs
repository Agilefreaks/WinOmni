namespace Omnipaste.Services.Connectivity
{
    using System;

    public class ConnectivityChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; private set; }

        public ConnectivityChangedEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}