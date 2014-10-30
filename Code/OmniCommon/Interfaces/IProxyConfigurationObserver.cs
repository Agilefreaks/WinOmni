namespace OmniCommon.Interfaces
{
    public interface IProxyConfigurationObserver
    {
        void OnConfigurationChanged(ProxyConfiguration proxyConfiguration);
    }
}