namespace OmniApi.Resources
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using Ninject;
    using OmniApi.Dto;
    using OmniCommon;
    using OmniApi.Support;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;

    public abstract class ResourceWithAuthorization<T> : Resource<T>
        where T : class
    {
        #region Constructors and Destructors

        protected ResourceWithAuthorization(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        #endregion

        #region Public Properties

        public string AccessToken
        {
            get
            {
                if (string.IsNullOrEmpty(TokenDto.AccessToken))
                {
                    var callingMethodName = new StackFrame(1).GetMethod().Name;
                    ExceptionReporter.Instance.Report(
                        new Exception(string.Format("AccessToken is empty when calling {0}", callingMethodName)));
                }
                return string.Concat("bearer ", TokenDto.AccessToken);
            }
        }

        [Inject]
        public IOAuth2 OAuth2 { get; set; }

        [Inject]
        public IHttpResponseMessageHandler ResponseHandler { get; set; }

        public TokenDto TokenDto
        {
            get
            {
                return new TokenDto(ConfigurationService.AccessToken, ConfigurationService.RefreshToken);
            }
        }

        #endregion

        #region Protected Methods and Operators
        
        protected override HttpClient CreateHttpClient()
        {
            if (OAuth2 == null)
            {
                throw new ArgumentNullException("OAuth2", "Service not present");
            }
            if (ResponseHandler == null)
            {
                throw new ArgumentNullException("ResponseHandler", "Service not present");
            }

            var baseAddress = new Uri(ConfigurationService[ConfigurationProperties.BaseUrl]);
            var handler = new HttpClientWithAuthorizationHandler(OAuth2, TokenDto, ResponseHandler)
            {
                InnerHandler = CreateHttpHandler()
            };
            return new HttpClient(handler) { BaseAddress = baseAddress };
        }

        #endregion
    }
}