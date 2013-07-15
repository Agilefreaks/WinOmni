using System.Runtime.InteropServices;

namespace Omnipaste.Services.Connectivity
{
    public class ConnectivityHelper
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        /// <summary>
        /// Returns whether Internet is connected
        /// </summary>
        /// <returns>bool</returns>
        public static bool InternetConnected
        {
            get
            {
                int Description = 0;
                return InternetGetConnectedState(out Description, 0);
            }
        }
    }
}
