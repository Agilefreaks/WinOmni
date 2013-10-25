namespace OmniApi.Resources
{
    using RestSharp;

    public abstract class ApiResource
    {
        public IRestClient RestClient { get; set; }

        protected string BaseUrl { get; set; }

        public static T Build<T>(string baseUrl, IRestClient restClient) where T : ApiResource, new()
        {
            T result = new T() { BaseUrl = baseUrl, RestClient = restClient };

            return result;
        }
    }
}