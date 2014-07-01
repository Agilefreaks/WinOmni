namespace Clipboard.Models
{
    using System;
    using Clipboard.Enums;
    
    public class Clipping
    {
        public enum ClippingType
        {
            PhoneNumber,
            WebSite,
            Address,
            Unknown
        }

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
            Content = content;
            Identifier = identifier;
        }

        #endregion

        #region Public Properties

        public string Content { get; set; }
        
        public string Identifier { get; set; }

        public ClippingType Type { get; set; }

        public ClippingSourceEnum Source { get; set; }

        public bool IsLink
        {
            get
            {
                return Type == ClippingType.WebSite || Uri.IsWellFormedUriString(Content, UriKind.RelativeOrAbsolute);
            }
        }

        #endregion
    }
}