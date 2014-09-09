namespace Omnipaste.Services.Connectivity
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    public class ConnectivityNotifyService : IConnectivityNotifyService
    {
        public IConnectivityHelper ConnectivityHelper { get; set; }

        private static readonly object Lock = new object();
        private Timer _timer;

        public bool PreviouslyConnected { get; set; }

        public bool CurrentlyConnected
        {
            get
            {
                return ConnectivityHelper.InternetConnected;
            }
        }

        public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

        public ConnectivityNotifyService(IConnectivityHelper connectivityHelper)
        {
            ConnectivityHelper = connectivityHelper;
        }

        public void Start()
        {
            PreviouslyConnected = ConnectivityHelper.InternetConnected;

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
                if (CurrentlyConnected != PreviouslyConnected)
                {
                    OnConnectivityChanged(CurrentlyConnected);
                    PreviouslyConnected = CurrentlyConnected;
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