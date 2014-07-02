namespace Clipboard.Models
{
    using System;

    public class Clipping
    {
        public enum ClippingSourceEnum
        {
            Local,
            Cloud
        }

        public enum ClippingTypeEnum
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

        public ClippingTypeEnum Type { get; set; }

        public ClippingSourceEnum Source { get; set; }

        public bool IsLink
        {
            get
            {
                return Type == ClippingTypeEnum.WebSite || Uri.IsWellFormedUriString(Content, UriKind.Absolute);
            }
        }

        #endregion
    }
}