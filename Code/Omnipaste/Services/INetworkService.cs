namespace Omnipaste.Services
{
    using OmniCommon;

    public interface INetworkService
    {
        bool CanPingHome(ProxyConfiguration? proxyConfiguration = null);

        void PingHome(ProxyConfiguration? proxyConfiguration = null);
    }
}