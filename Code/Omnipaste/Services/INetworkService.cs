namespace Omnipaste.Services
{
    using OmniCommon.Models;

    public interface INetworkService
    {
        bool CanPingHome(ProxyConfiguration proxyConfiguration = null);

        void PingHome(ProxyConfiguration proxyConfiguration = null);
    }
}