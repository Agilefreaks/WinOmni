namespace OmniCommon.Models
{
    public class OmniMessage
    {
        public OmniMessageTypeEnum Provider { get; set; }
    }

    public enum OmniMessageTypeEnum
    {
        Clipboard,
        PhoneNumber,
        Notification
    }
}