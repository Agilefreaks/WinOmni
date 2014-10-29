namespace OmniApi.Resources.v1
{
    public class AuthorizationRequest
    {
        #region Constructors and Destructors

        public AuthorizationRequest(GrantTypeEnum grantType, string clientId)
        {
            GrantType = grantType.ToString();
            ClientId = clientId;
        }

        #endregion

        #region Public Properties

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Code { get; set; }

        public string GrantType { get; set; }

        public string RefreshToken { get; set; }

        #endregion
    }
}