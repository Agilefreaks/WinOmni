namespace OmniApi.Models
{
    public class Clipping
    {
        public string Token { get; set; }

        public string Content { get; set; }

        public Clipping()
        {
        }

        public Clipping(string token, string content)
        {
            Token = token;
            Content = content;
        }
    }
}