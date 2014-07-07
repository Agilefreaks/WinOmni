namespace OmniApi.Resources.v1
{
    using System;
    using System.Configuration;
    using OmniApi.Models;
    using OmniCommon;
    using Refit;

    public class OAuth2 : Resource<OAuth2.IOAuth2Api>, IOAuth2
    {
        #region Fields

        private readonly string _clientId;

        #endregion

        #region Constructors and Destructors

        public OAuth2()
        {
            _clientId = ConfigurationManager.AppSettings[ConfigurationProperties.ClientId];
        }

        #endregion

        #region Enums

        public enum GrantTypeEnum
        {
            authorization_code,

            refresh_token,

            client_credentials
        }

        #endregion

        #region Interfaces

        [ColdObservable]
        public interface IOAuth2Api
        {
            #region Public Methods and Operators

            [Post("/oauth2/token")]
            IObservable<Token> Create([Body] AuthorizationRequest request);

            #endregion
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Token> Create(string authorizationCode)
        {
            return ResourceApi.Create(new AuthorizationRequest(GrantTypeEnum.authorization_code, _clientId) { Code = authorizationCode });
        }

        public IObservable<Token> Refresh(string refreshToken)
        {
            return ResourceApi.Create(new AuthorizationRequest(GrantTypeEnum.refresh_token, _clientId) { RefreshToken = refreshToken });
        }

        #endregion

        public class AuthorizationRequest
        {
            public AuthorizationRequest(GrantTypeEnum grantType, string clientId)
            {
                GrantType = grantType.ToString();
                ClientId = clientId;
            }

            #region Public Properties

            public string ClientId { get; set; }

            public string ClientSecret { get; set; }

            public string Code { get; set; }

            public string GrantType { get; set; }

            public string RefreshToken { get; set; }

            #endregion
        }
    }
}