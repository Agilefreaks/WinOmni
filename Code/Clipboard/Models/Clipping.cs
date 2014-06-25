namespace Clipboard.Models
{
    using Clipboard.Enums;

    public class Clipping
    {
        public string Content { get; set; }

        public ClippingSourceEnum Source { get; set; }

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