namespace OmniApi.Models
{
    public class Token
    {
        #region Constructors and Destructors

        public Token()
        {
        }

        public Token(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        #endregion

        #region Public Properties

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        #endregion
    }
}