namespace OmniApi.Resources.v1
{
    using System;
    using System.Configuration;
    using System.Net.Http;
    using OmniApi.Models;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Refit;

    public class OAuth2 : Resource<IOAuth2Api>, IOAuth2
    {
        #region Fields

        private readonly string _clientId;

        #endregion

        #region Constructors and Destructors

        public OAuth2(IWebProxyFactory webProxyFactory)
            :base(webProxyFactory)
        {
            _clientId = ConfigurationManager.AppSettings[ConfigurationProperties.ClientId];
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Token> Create(string authorizationCode)
        {
            return
                ResourceApi.Create(
                    new AuthorizationRequest(GrantTypeEnum.authorization_code, _clientId) { Code = authorizationCode });
        }

        public IObservable<Token> Refresh(string refreshToken)
        {
            return
                ResourceApi.Create(
                    new AuthorizationRequest(GrantTypeEnum.refresh_token, _clientId) { RefreshToken = refreshToken });
        }

        #endregion

        #region Methods

        protected override IOAuth2Api CreateResourceApi(HttpClient httpClient)
        {
            return RestService.For<IOAuth2Api>(httpClient);
        }

        #endregion
    }
}