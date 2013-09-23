namespace Omnipaste.OmniClipboard.Infrastructure.Api.Resources
{
    using Omnipaste.OmniClipboard.Infrastructure.Services;
    using RestSharp;

    public abstract class ApiResource
    {
        public IRestClient RestClient { get; set; }

        protected string BaseUrl { get; set; }

        protected string Version { get; set; }

        public string ApiUrl
        {
            get { return string.Format("{0}/{1}", BaseUrl, Version); }
        }

        protected ApiResource(IConfigurationManager configurationManager, IRestClient restClient)
        {
            BaseUrl = configurationManager.AppSettings["apiUrl"];
            Version = "v1";
            restClient.BaseUrl = ApiUrl;
            this.RestClient = restClient;
        }
    }
}