namespace Omnipaste.Models
{
    using System;
    using Clipboard.Models;

    public class ClippingModel : BaseModel
    {
        public string Content { get; set; }

        public string Identifier { get; set; }

        public DateTime Time { get; set; }

        public Clipping.ClippingTypeEnum Type { get; set; }

        public Clipping.ClippingSourceEnum Source { get; set; }

        public bool IsLink
        {
            get
            {
                return Type == Clipping.ClippingTypeEnum.Url;
            }
        }

        public ClippingModel()
        {
        }

        public ClippingModel(Clipping clipping)
        {
            UniqueId = clipping.UniqueId;
            Time = clipping.Time;
            Content = clipping.Content;
            Identifier = clipping.Identifier;
            Type = clipping.Type;
            Source = clipping.Source;
        }
    }
}
