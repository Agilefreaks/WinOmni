namespace OmniCommon.Models
{
    public class OmniMessage
    {
        public OmniMessageTypeEnum Type { get; set; }
    }

    public enum OmniMessageTypeEnum
    {
        Clipping,
        PhoneNumber
    }
}