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
            access_token = accessToken;
            refresh_token = refreshToken;
        }

        #endregion

        #region Public Properties

        public string access_token { get; set; }

        public string refresh_token { get; set; }

        #endregion
    }
}