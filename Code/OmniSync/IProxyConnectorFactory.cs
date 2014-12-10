namespace OmniSync
{
    using OmniCommon.Models;
    using SuperSocket.ClientEngine;

    public interface IProxyConnectorFactory
    {
        IProxyConnector Create(ProxyConfiguration proxyConfiguration);
    }
}