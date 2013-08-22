namespace Omnipaste.OmniClipboard.Infrastructure.Api
{
    public class Clipping
    {
        public const string ResourceKey = "clippings";

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
