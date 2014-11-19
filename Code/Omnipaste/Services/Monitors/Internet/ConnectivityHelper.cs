namespace Omnipaste.Services.Monitors.Internet
{
    using System;
    using System.Reactive.Linq;
    using System.Runtime.InteropServices;
    using OmniCommon.Helpers;

    public class ConnectivityHelper : IConnectivityHelper
    {
        #region Static Fields

        public static readonly TimeSpan DefaultCheckInterval = TimeSpan.FromSeconds(3);

        #endregion

        #region Fields

        private readonly TimeSpan _checkInterval;

        #endregion

        #region Constructors and Destructors

        public ConnectivityHelper()
            : this(DefaultCheckInterval)
        {
        }

        public ConnectivityHelper(TimeSpan checkInterval)
        {
            _checkInterval = checkInterval;
        }

        #endregion

        #region Public Properties

        public IObservable<bool> InternetConnectivityObservable
        {
            get
            {
                return
                    Observable.Timer(TimeSpan.Zero, _checkInterval, SchedulerProvider.Default)
                        .Select(_ => IsInternetConnected());
            }
        }

        #endregion

        #region Methods

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        private static bool IsInternetConnected()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }

        #endregion
    }
}