namespace OmniApi.Resources
{
    using System.Diagnostics;
    using RestSharp;

    public abstract class ApiResource
    {
        public IRestClient RestClient { get; set; }

        public string ApiUrl
        {
            get { return string.Format("{0}/{1}", this.BaseUrl, this.Version); }
        }

        protected string BaseUrl { get; set; }

        protected string Version
        {
            get
            {
                return "v1";
            }
        }

        public static T Build<T>(string baseUrl, IRestClient restClient) where T : ApiResource, new()
        {
            T result = new T() { BaseUrl = baseUrl, RestClient = restClient };

            return result;
        }
    }
}