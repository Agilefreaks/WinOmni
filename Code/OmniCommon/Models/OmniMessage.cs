namespace OmniCommon.Models
{
    using System.Collections.Generic;

    public class OmniMessage
    {
        public OmniMessageProviderEnum Provider { get; set; }

        public string Type { get; set; }

        public Dictionary<string, string> Payload { get; set; } 

        public OmniMessage()
        {
            Payload = new Dictionary<string, string>();
        }

        public OmniMessage(OmniMessageProviderEnum provider)
        {
            Provider = provider;
        }

        public string GetPayload(string key)
        {
            string result = null;
            if (Payload != null && Payload.ContainsKey(key))
            {
                result = Payload[key];
            }

            return result;
        }
    }
}