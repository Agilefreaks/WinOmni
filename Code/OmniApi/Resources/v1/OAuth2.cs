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

        public enum GrantType
        {
            authorization_code,

            refresh_token,

            client_credentials
        }

        #endregion

        #region Interfaces

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
            return ResourceApi.Create(new AuthorizationRequest(GrantType.authorization_code, _clientId) { code = authorizationCode });
        }

        public IObservable<Token> Refresh(string refreshToken)
        {
            return ResourceApi.Create(new AuthorizationRequest(GrantType.refresh_token, _clientId) { refresh_token = refreshToken });
        }

        #endregion

        public class AuthorizationRequest
        {
            public AuthorizationRequest(GrantType grantType, string clientId)
            {
                grant_type = grantType.ToString();
                client_id = clientId;
            }

            #region Public Properties

            public string client_id { get; set; }

            public string client_secret { get; set; }

            public string code { get; set; }

            public string grant_type { get; set; }

            public string refresh_token { get; set; }

            #endregion
        }
    }
}