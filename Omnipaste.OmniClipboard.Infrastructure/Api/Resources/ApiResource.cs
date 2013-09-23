namespace Omnipaste.OmniClipboard.Infrastructure.Api.Resources
{
    using Omnipaste.OmniClipboard.Infrastructure.Services;
    using RestSharp;

    public abstract class ApiResource
    {
        public IRestClient RestClient { get; set; }

        protected string BaseUrl { get; set; }

        public string ApiUrl
        {
            get { return string.Format("{0}/{1}", this.BaseUrl, Users.Version); }
        }

        protected ApiResource(IConfigurationManager configurationManager, IRestClient restClient)
        {
            this.BaseUrl = configurationManager.AppSettings["apiUrl"];
            restClient.BaseUrl = this.ApiUrl;
            this.RestClient = restClient;
        }
    }
}