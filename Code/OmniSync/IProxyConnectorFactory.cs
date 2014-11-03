namespace OmniSync
{
    using OmniCommon;
    using SuperSocket.ClientEngine;

    public interface IProxyConnectorFactory
    {
        IProxyConnector Create(ProxyConfiguration proxyConfiguration);
    }
}