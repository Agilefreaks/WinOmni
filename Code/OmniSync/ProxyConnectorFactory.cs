namespace OmniSync
{
    using System.Net;
    using OmniCommon;
    using OmniSync.ClientEngine;
    using SuperSocket.ClientEngine;

    public class ProxyConnectorFactory : IProxyConnectorFactory
    {
        public IProxyConnector Create(ProxyConfiguration proxyConfiguration)
        {
            IProxyConnector result;
            switch (proxyConfiguration.Type)
            {
                case ProxyTypeEnum.Http:
                    result = CreateHttpProxyConnector(proxyConfiguration);
                    break;
                case ProxyTypeEnum.Socks4:
                    result = CreateSocks4ProxyConnector(proxyConfiguration);
                    break;
                case ProxyTypeEnum.Socks4A:
                    result = CreateSocks4AProxyConnector(proxyConfiguration);
                    break;
                case ProxyTypeEnum.Socks5:
                    result = CreateSocks5ProxyConnector(proxyConfiguration);
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        private static IProxyConnector CreateSocks5ProxyConnector(ProxyConfiguration proxyConfiguration)
        {
            return new Socks5Connector(
                new DnsEndPoint(proxyConfiguration.Address, proxyConfiguration.Port),
                proxyConfiguration.Username ?? string.Empty,
                proxyConfiguration.Password ?? string.Empty);
        }

        private static IProxyConnector CreateSocks4AProxyConnector(ProxyConfiguration proxyConfiguration)
        {
            return new Socks4aConnector(
                new DnsEndPoint(proxyConfiguration.Address, proxyConfiguration.Port),
                proxyConfiguration.Username ?? string.Empty);
        }

        private static IProxyConnector CreateSocks4ProxyConnector(ProxyConfiguration proxyConfiguration)
        {
            return new Socks4Connector(
                new DnsEndPoint(proxyConfiguration.Address, proxyConfiguration.Port),
                proxyConfiguration.Username ?? string.Empty);
        }

        private static IProxyConnector CreateHttpProxyConnector(ProxyConfiguration proxyConfiguration)
        {
            return new HttpConnectProxy(new DnsEndPoint(proxyConfiguration.Address, proxyConfiguration.Port),
                proxyConfiguration.Username ?? string.Empty,
                proxyConfiguration.Password ?? string.Empty);
        }
    }
}