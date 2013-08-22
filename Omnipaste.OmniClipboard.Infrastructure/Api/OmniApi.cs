using Omnipaste.OmniClipboard.Core.Api.Resources;
using Omnipaste.OmniClipboard.Infrastructure.Api.Resources;

namespace Omnipaste.OmniClipboard.Infrastructure.Api
{
    using System.Configuration;
    using Omnipaste.OmniClipboard.Core.Api;

    public class OmniApi : IOmniApi
    {
        public const string Version = "v1";

        public string ApiUrl { get; private set; }

        public string ApiKey { get; set; }

        public IClippings Clippings
        {
            get { return new Clippings(ApiUrl, Version, ApiKey); }
        }

        public OmniApi()
        {
            ApiUrl = ConfigurationManager.AppSettings["apiUrl"];
        }
    }
}
