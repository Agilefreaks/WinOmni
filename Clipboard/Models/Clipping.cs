namespace Clipboard.Models
{
    public class Clipping
    {
        public string Content { get; set; }

        public Clipping()
            : this(string.Empty)
        {
        }

        public Clipping(string content)
        {
            Content = content;
        }
    }
}