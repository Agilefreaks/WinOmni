namespace OmniApi.Resources.v1
{
    using System;
    using System.Configuration;
    using OmniApi.Models;
    using OmniCommon;
    using Refit;

    public class OAuth2 : Resource<IOAuth2Api>, IOAuth2
    {
        #region Fields

        private readonly string _clientId;

        #endregion

        #region Constructors and Destructors

        public OAuth2()
            : base(CreateResourceApi())
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

        private static IOAuth2Api CreateResourceApi()
        {
            return RestService.For<IOAuth2Api>(ConfigurationManager.AppSettings[ConfigurationProperties.BaseUrl]);
        }

        #endregion
    }
}