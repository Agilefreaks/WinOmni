namespace OmniApi.Resources
{
    public class Authorization
    {
        string AccessToken { get; set; }
        
        string RefreshToken { get; set; }

        string TokenType { get; set; }

        int ExpiresIn { get; set; }
    }
}