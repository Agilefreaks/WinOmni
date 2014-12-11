namespace Omnipaste.Services
{
    using System;
    using System.Net;
    using System.Net.Cache;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class NetworkService : INetworkService
    {
        private const int TimeoutInMilliseconds = 100000;

        private readonly IConfigurationService _configurationService;

        private readonly IWebProxyFactory _webProxyFactory;

        public NetworkService(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
        {
            _configurationService = configurationService;
            _webProxyFactory = webProxyFactory;
        }

        public bool CanPingHome(ProxyConfiguration proxyConfiguration = null)
        {
            var result = true;
            try
            {
                PingHome(proxyConfiguration);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public void PingHome(ProxyConfiguration proxyConfiguration = null)
        {
            proxyConfiguration = proxyConfiguration ?? _configurationService.ProxyConfiguration;
            SimpleLogger.Log("Pinging home");
            var webRequest = WebRequest.Create(new Uri(_configurationService[ConfigurationProperties.PingEndpoint]));
            webRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            webRequest.Timeout = TimeoutInMilliseconds;
            SimpleLogger.Log("Given proxy type is: " + _configurationService.ProxyConfiguration.Type);
            SimpleLogger.Log("Given proxy host is: " + _configurationService.ProxyConfiguration.Address);
            SimpleLogger.Log("Given proxy port is: " + _configurationService.ProxyConfiguration.Port);
            SimpleLogger.Log("Proxy username is empty: " + string.IsNullOrWhiteSpace(_configurationService.ProxyConfiguration.Username));
            SimpleLogger.Log("Proxy password is empty: " + string.IsNullOrWhiteSpace(_configurationService.ProxyConfiguration.Password));
            webRequest.Proxy = _webProxyFactory.CreateForConfiguration(proxyConfiguration);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            SimpleLogger.Log("Result of ping is: " + webResponse.StatusCode);
            if (webResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Response code not OK", null, WebExceptionStatus.UnknownError, webResponse);
            }
        }
    }
}