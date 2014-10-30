namespace OmniCommon.Interfaces
{
    using System.Net;
    using OmniCommon;

    public interface IWebProxyFactory
    {
        IWebProxy CreateForConfiguration(ProxyConfiguration proxyConfiguration);

        IWebProxy CreateFromAppConfiguration();
    }
}