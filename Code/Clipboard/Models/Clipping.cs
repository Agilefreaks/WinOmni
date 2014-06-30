namespace Clipboard.Models
{
    using System;
    using Clipboard.Enums;
    
    public class Clipping
    {
        #region Constructors and Destructors

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

        #endregion

        #region Public Properties

        public string content { get; set; }
        
        public string identifier { get; set; }

        public string type { get; set; }

        public ClippingSourceEnum source { get; set; }

        public bool IsLink
        {
            get
            {
                return type == "web_site" || Uri.IsWellFormedUriString(content, UriKind.Absolute);
            }
        }

        #endregion
    }
}