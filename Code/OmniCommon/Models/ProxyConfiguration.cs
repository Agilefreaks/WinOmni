namespace OmniCommon.Models
{
    using System;

    [Serializable]
    public class ProxyConfiguration
    {
        public ProxyTypeEnum Type { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public static ProxyConfiguration Empty()
        {
            return new ProxyConfiguration();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof (ProxyConfiguration))
            {
                return false;
            }

            var otherProxyConfig = (ProxyConfiguration) obj;

            return Type == otherProxyConfig.Type
                   && Address == otherProxyConfig.Address
                   && Port == otherProxyConfig.Port
                   && Username == otherProxyConfig.Username
                   && Password == otherProxyConfig.Password;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Type, Address, Port, Username, Password).GetHashCode();
        }
    }
}