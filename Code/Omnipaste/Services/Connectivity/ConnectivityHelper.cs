namespace Omnipaste.Services.Connectivity
{
    using System.Runtime.InteropServices;

    public class ConnectivityHelper
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        public static bool InternetConnected
        {
            get
            {
                int description;
                return InternetGetConnectedState(out description, 0);
            }
        }
    }
}