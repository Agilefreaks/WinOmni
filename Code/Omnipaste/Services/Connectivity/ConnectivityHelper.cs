using System.Runtime.InteropServices;

namespace Omnipaste.Services.Connectivity
{
    public interface IConnectivityHelper
    {
        /// <summary>
        /// Returns whether Internet is connected
        /// </summary>
        /// <returns>bool</returns>
        bool InternetConnected { get; }
    }

    public class ConnectivityHelper : IConnectivityHelper
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        /// <summary>
        /// Returns whether Internet is connected
        /// </summary>
        /// <returns>bool</returns>
        public bool InternetConnected
        {
            get
            {
                int Description = 0;
                return InternetGetConnectedState(out Description, 0);
            }
        }
    }
}
