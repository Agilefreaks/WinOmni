namespace Omnipaste.Models
{
    using Clipboard.Models;

    public class ClippingModel
    {
        public string UniqueId { get; set; }

        public string Content { get; set; }

        public string Identifier { get; set; }

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
            Content = clipping.Content;
            Identifier = clipping.Identifier;
            Type = clipping.Type;
            Source = clipping.Source;
        }
    }
}
