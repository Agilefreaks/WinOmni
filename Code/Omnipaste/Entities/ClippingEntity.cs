namespace Omnipaste.Entities
{
    using Clipboard.Dto;
    using OmniUI.Entities;

    public class ClippingEntity : Entity
    {
        public string Content { get; set; }

        public string DeviceId { get; set; }

        public bool IsStarred { get; set; }

        public ClippingDto.ClippingTypeEnum Type { get; set; }

        public ClippingDto.ClippingSourceEnum Source { get; set; }

        public bool IsLink
        {
            get
            {
                return Type == ClippingDto.ClippingTypeEnum.Url;
            }
        }

        public ClippingEntity()
        {
        }

        public ClippingEntity(ClippingDto clippingDto)
            : this()
        {
            Id = clippingDto.Id;
            Content = clippingDto.Content;
            DeviceId = clippingDto.DeviceId;
            Type = clippingDto.Type;
            Source = clippingDto.Source;
        }
    }
}
