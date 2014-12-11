namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps.ProxyDetection
{
    using System;
    using System.Net;
    using OmniCommon;
    using OmniCommon.Models;
    using OmniCommon.Interfaces;

    public abstract class SystemWebProxyConfigurationDetector : IProxyConfigurationDetector
    {
        #region Fields

        protected readonly IConfigurationService ConfigurationService;

        #endregion

        #region Constructors and Destructors

        protected SystemWebProxyConfigurationDetector(IConfigurationService configurationService)
        {
            ConfigurationService = configurationService;
        }

        #endregion

        #region Public Properties

        public abstract ProxyTypeEnum ProxyType { get; }

        #endregion

        #region Public Methods and Operators

        public ProxyConfiguration Detect()
        {
            ProxyConfiguration result = null;
            var pingEndpoint = GetPingEndpoint();
            SimpleLogger.Log("Checking for proxy type: " + ProxyType);
            var systemWebProxy = WebRequest.GetSystemWebProxy();
            var proxy = systemWebProxy.GetProxy(new Uri(pingEndpoint));
            SimpleLogger.Log("Obtained proxy address: " + proxy);
            if (proxy.ToString() != pingEndpoint)
            {
                SimpleLogger.Log("Looks like we found a new proxy server");
                var proxyConfiguration = new ProxyConfiguration
                                             {
                                                 Address = proxy.DnsSafeHost,
                                                 Port = proxy.Port,
                                                 Type = ProxyType
                                             };
                result = proxyConfiguration;
            }

            return result;
        }

        #endregion

        #region Methods

        protected virtual string GetPingEndpoint()
        {
            return ConfigurationService[ConfigurationProperties.PingEndpoint];
        }

        #endregion
    }
}