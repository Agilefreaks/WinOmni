namespace Omnipaste.Services.Monitors.Internet
{
    using System.Runtime.InteropServices;

    public class ConnectivityHelper : IConnectivityHelper
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        public bool InternetConnected
        {
            get
            {
                int description;
                return InternetGetConnectedState(out description, 0);
            }
        }
    }
}