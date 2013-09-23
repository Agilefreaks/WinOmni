namespace Omnipaste.OmniClipboard.Infrastructure.Services
{
    using System.Collections.Specialized;

    public interface IConfigurationManager
    {
        NameValueCollection AppSettings { get; }
    }
}