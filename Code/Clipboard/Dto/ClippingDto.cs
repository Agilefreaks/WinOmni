namespace Clipboard.Dto
{
    public class ClippingDto
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

        public ClippingDto()
            : this(string.Empty, string.Empty)
        {
        }

        public ClippingDto(string content)
            : this(content, string.Empty)
        {
        }

        public ClippingDto(string content, string deviceId)
        {
            Content = content;
            DeviceId = deviceId;
        }

        public string Id { get; set; }

        public string Content { get; set; }
        
        public string DeviceId { get; set; }

        public ClippingTypeEnum Type { get; set; }

        public ClippingSourceEnum Source { get; set; }
    }
}