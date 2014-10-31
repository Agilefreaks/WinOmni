namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Net;
    using System.Net.Cache;
    using OmniCommon;
    using OmniCommon.Interfaces;

    public class VerifyConnectivity : SynchronousStepBase
    {
        private const int TimeoutInMilliseconds = 100000;

        private readonly IConfigurationService _configurationService;

        private readonly IWebProxyFactory _webProxyFactory;

        public VerifyConnectivity(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
        {
            _configurationService = configurationService;
            _webProxyFactory = webProxyFactory;
        }

        protected override IExecuteResult ExecuteSynchronously()
        {
            var executeResult = new ExecuteResult();

            var webRequest = WebRequest.Create(new Uri(_configurationService[ConfigurationProperties.PingEndpoint]));
            webRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            webRequest.Timeout = TimeoutInMilliseconds;
            webRequest.Proxy = _webProxyFactory.CreateForConfiguration(_configurationService.ProxyConfiguration);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            executeResult.State = webResponse.StatusCode == HttpStatusCode.OK
                                      ? SimpleStepStateEnum.Successful
                                      : SimpleStepStateEnum.Failed;

            return executeResult;
        }
    }
}