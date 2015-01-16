namespace Omnipaste.Models
{
    using Clipboard.Models;

    public class ClippingModel : BaseModel
    {
        public string Id { get; set; }

        public string Content { get; set; }

        public string DeviceId { get; set; }

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
            : this()
        {
            Id = clipping.Id;
            Content = clipping.Content;
            DeviceId = clipping.DeviceId;
            Type = clipping.Type;
            Source = clipping.Source;
        }
    }
}
