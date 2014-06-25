namespace Clipboard.Models
{
    using System;
    using Clipboard.Enums;
    
    public class Clipping
    {
        #region Constructors and Destructors

        public Clipping()
            : this(string.Empty)
        {
        }

        public Clipping(string content)
        {
            Content = content;
        }

        #endregion

        #region Public Properties

        public string Content { get; set; }

        public string Type { get; set; }

        public ClippingSourceEnum Source { get; set; }

        public bool IsLink
        {
            get
            {
                return Type == "web_site" || Uri.IsWellFormedUriString(Content, UriKind.RelativeOrAbsolute);
            }
        }

        #endregion
    }
}