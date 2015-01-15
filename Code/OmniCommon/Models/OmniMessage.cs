namespace OmniCommon.Models
{
    public class OmniMessage
    {
        public OmniMessageProviderEnum Provider { get; set; }

        public OmniMessage()
        {
        }

        public OmniMessage(OmniMessageTypeEnum provider)
        {
            Provider = provider;
        }
    }
}