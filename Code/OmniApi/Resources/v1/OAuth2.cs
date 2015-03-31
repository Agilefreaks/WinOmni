namespace OmniApi.Resources.v1
{
    using System;
    using System.Net.Http;
    using OmniApi.Dto;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Refit;

    public class OAuth2 : Resource<IOAuth2Api>, IOAuth2
    {
        #region Fields

        private readonly string _clientId;

        #endregion

        #region Constructors and Destructors

        public OAuth2(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            :base(configurationService, webProxyFactory)
        {
            _clientId = configurationService[ConfigurationProperties.ClientId];
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<TokenDto> Create(string authorizationCode)
        {
            return
                ResourceApi.Create(
                    new AuthorizationRequest(GrantTypeEnum.authorization_code, _clientId) { Code = authorizationCode });
        }

        public IObservable<TokenDto> Refresh(string refreshToken)
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