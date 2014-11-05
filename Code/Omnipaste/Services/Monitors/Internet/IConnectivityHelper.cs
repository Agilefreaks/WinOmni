namespace Omnipaste.Services.Monitors.Internet
{
    public interface IConnectivityHelper
    {
        bool InternetConnected { get; }
    }
}