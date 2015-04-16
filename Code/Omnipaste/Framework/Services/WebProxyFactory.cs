namespace Omnipaste.Framework.Services
{
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using LandonKey.SocksWebProxy;
    using LandonKey.SocksWebProxy.Proxy;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class WebProxyFactory : IWebProxyFactory
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        #endregion

        #region Constructors and Destructors

        public WebProxyFactory(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region Public Methods and Operators

        public IWebProxy CreateFromAppConfiguration()
        {
            return CreateForConfiguration(_configurationService.ProxyConfiguration);
        }

        public IWebProxy CreateForConfiguration(ProxyConfiguration proxyConfiguration)
        {
            IWebProxy result;
            switch (proxyConfiguration.Type)
            {
                case ProxyTypeEnum.Http:
                    result = GetHttpProxy(proxyConfiguration);
                    break;
                case ProxyTypeEnum.Socks4:
                case ProxyTypeEnum.Socks4A:
                case ProxyTypeEnum.Socks5:
                    result = GetSocksProxy(proxyConfiguration);
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        #endregion

        #region Methods

        private static int GetFreeHttpPort()
        {
            int result;

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // Passing 0 here will tell the system to assign any free port to the socket.
                socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
                result = ((IPEndPoint)socket.LocalEndPoint).Port;
            }

            return result;
        }

        private static IWebProxy GetHttpProxy(ProxyConfiguration proxyConfiguration)
        {
            var networkCredential = new NetworkCredential(proxyConfiguration.Username, proxyConfiguration.Password);
            return new WebProxy(proxyConfiguration.Address, proxyConfiguration.Port) { Credentials = networkCredential };
        }

        private static IWebProxy GetSocksProxy(ProxyConfiguration proxyConfiguration)
        {
            var hostAddresses = Dns.GetHostAddresses(proxyConfiguration.Address);
            var proxyConfig = new ProxyConfig
                                  {
                                      HttpPort = GetFreeHttpPort(),
                                      HttpAddress = IPAddress.Loopback,
                                      SocksAddress = hostAddresses.First(),
                                      SocksPort = proxyConfiguration.Port,
                                      Version =
                                          proxyConfiguration.Type == ProxyTypeEnum.Socks5
                                              ? ProxyConfig.SocksVersion.Five
                                              : ProxyConfig.SocksVersion.Four
                                  };

            return new SocksWebProxy(proxyConfig);
        }

        #endregion
    }
}