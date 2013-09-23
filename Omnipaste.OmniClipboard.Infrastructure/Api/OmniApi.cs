using Omnipaste.OmniClipboard.Core.Api.Resources;

namespace Omnipaste.OmniClipboard.Infrastructure.Api
{
    using System.Configuration;
    using Omnipaste.OmniClipboard.Core.Api;

    public class OmniApi : IOmniApi
    {
        private string _apiKey;

        public const string Version = "v1";

        public string BaseUrl { get; set; }

        public string ApiKey
        {
            get
            {
                return _apiKey;
            }
            set
            {
                _apiKey = value;
                Clippings.ApiKey = _apiKey;
            }
        }

        public string ApiUrl
        {
            get { return string.Format("{0}/{1}", BaseUrl, Version); }
        }

        public IClippings Clippings { get; set; }

        public OmniApi(IClippings clippings)
        {
            BaseUrl = ConfigurationManager.AppSettings["apiUrl"];
            
            Clippings = clippings;
        }
    }
}
