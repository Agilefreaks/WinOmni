namespace Omnipaste.Entities
{
    using Clipboard.Models;
    using OmniUI.Entities;

    public class ClippingEntity : Entity
    {
        public string Content { get; set; }

        public string DeviceId { get; set; }

        public bool IsStarred { get; set; }

        public Clipping.ClippingTypeEnum Type { get; set; }

        public Clipping.ClippingSourceEnum Source { get; set; }

        public bool IsLink
        {
            get
            {
                return Type == Clipping.ClippingTypeEnum.Url;
            }
        }

        public ClippingEntity()
        {
        }

        public ClippingEntity(Clipping clipping)
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
