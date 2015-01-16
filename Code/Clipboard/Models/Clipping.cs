namespace Clipboard.Models
{
    using System;
    using Newtonsoft.Json;
    using OmniCommon.Helpers;

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
            Url,
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
            Time = TimeHelper.UtcNow;
            Content = content;
            Identifier = identifier;
        }

        #endregion

        #region Public Properties

        public string Id { get; set; }

        public string Content { get; set; }
        
        public string Identifier { get; set; }

        public DateTime Time { get; set; }

        public ClippingTypeEnum Type { get; set; }

        public ClippingSourceEnum Source { get; set; }

        #endregion
    }
}