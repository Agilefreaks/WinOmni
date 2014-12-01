namespace OmniCommon.Settings
{
    public class OmnipasteCredentials
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public OmnipasteCredentials()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
        }

        public OmnipasteCredentials(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}