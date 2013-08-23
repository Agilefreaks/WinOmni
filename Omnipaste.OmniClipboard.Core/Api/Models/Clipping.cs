namespace Omnipaste.OmniClipboard.Core.Api.Models
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
