namespace OmniApi.Models
{
    public class Token
    {
        #region Constructors and Destructors

        public Token()
        {
        }

        public Token(string accessToken, string rrefreshToken)
        {
            AccessToken = accessToken;
            RrefreshToken = rrefreshToken;
        }

        #endregion

        #region Public Properties

        public string AccessToken { get; set; }

        public string RrefreshToken { get; set; }

        #endregion
    }
}