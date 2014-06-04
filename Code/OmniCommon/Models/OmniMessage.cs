namespace OmniCommon.Models
{
    public class OmniMessage
    {
        public OmniMessageTypeEnum Provider { get; set; }

        public OmniMessage()
        {
        }

        public OmniMessage(OmniMessageTypeEnum provider)
        {
            Provider = provider;
        }
    }
}