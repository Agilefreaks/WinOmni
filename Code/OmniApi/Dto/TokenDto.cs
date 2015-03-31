namespace OmniApi.Dto
{
    public class TokenDto
    {
        public TokenDto()
        {
        }

        public TokenDto(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}