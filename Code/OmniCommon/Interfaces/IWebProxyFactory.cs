namespace OmniCommon.Interfaces
{
    using System.Net;
    using OmniCommon.Models;

    public interface IWebProxyFactory
    {
        IWebProxy CreateForConfiguration(ProxyConfiguration proxyConfiguration);

        IWebProxy CreateFromAppConfiguration();
    }
}