namespace Omnipaste.Services
{
    using System.Net;
    using OmniCommon;

    public interface IWebProxyFactory
    {
        IWebProxy CreateForConfiguration(ProxyConfiguration proxyConfiguration);
    }
}