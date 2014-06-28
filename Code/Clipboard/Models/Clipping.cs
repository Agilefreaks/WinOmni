namespace Clipboard.Models
{
    public class Clipping
    {
        public string content { get; set; }

        public string identifier { get; set; }

        public Clipping()
            : this(string.Empty, string.Empty)
        {
        }

        public Clipping(string content)
            : this(content, string.Empty)
        {
        }

        public Clipping(string content, string identifier)
        {
            this.content = content;
            this.identifier = identifier;
        }
    }
}