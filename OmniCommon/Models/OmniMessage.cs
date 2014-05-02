namespace OmniCommon.Models
{
    public class OmniMessage
    {
        public string Provider { get; set; }
    }

    public enum OmniMessageTypeEnum
    {
        Clipping,
        PhoneNumber
    }
}