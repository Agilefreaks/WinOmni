namespace OmniCommon
{
    using System;

    [Serializable]
    public struct ProxyConfiguration
    {
        public ProxyTypeEnum Type;

        public string Address;

        public int Port;

        public string Username;

        public string Password;

        public static ProxyConfiguration Empty()
        {
            return new ProxyConfiguration();
        }
    }
}