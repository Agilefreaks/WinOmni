﻿namespace Omnipaste.Services.Connectivity
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    public class ConnectivityNotifyService : IConnectivityNotifyService
    {
        private static readonly object Lock = new object();
        private Timer _timer;

        private bool PreviouslyConnected { get; set; }

        public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

        public void Start()
        {
            _timer = new Timer(Run, null, TimeSpan.FromTicks(0), TimeSpan.FromSeconds(5));
        }

        public void Stop()
        {
            ConnectivityChanged = null;
            _timer.Dispose();
        }

        protected void Run(object state)
        {
            lock (Lock)
            {
                if (ConnectivityHelper.InternetConnected != PreviouslyConnected)
                {
                    OnConnectivityChanged(ConnectivityHelper.InternetConnected);
                    PreviouslyConnected = ConnectivityHelper.InternetConnected;
                }
            }
        }

        protected virtual void OnConnectivityChanged(bool isConnected)
        {
            var handler = ConnectivityChanged;
            if (handler != null)
            {
                Action target = () => handler(this, new ConnectivityChangedEventArgs(isConnected));

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, target);
            }
        }
    }
}