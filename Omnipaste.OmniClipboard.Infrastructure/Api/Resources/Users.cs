namespace Omnipaste.OmniClipboard.Infrastructure.Api.Resources
{
    using OmniCommon.DataProviders;
    using Omnipaste.OmniClipboard.Core.Api.Resources;
    using Omnipaste.OmniClipboard.Infrastructure.Services;
    using RestSharp;

    public class Users : IUsers
    {
        public readonly string ResourceKey = "users";

        public const string Version = "v1";

        public IRestClient RestClient { get; set; }

        protected string BaseUrl { get; set; }

        public string ApiUrl
        {
            get { return string.Format("{0}/{1}", BaseUrl, Version); }
        }

        public Users(IConfigurationManager configurationManager, IRestClient restClient)
        {
            BaseUrl = configurationManager.AppSettings["apiUrl"];
            restClient.BaseUrl = ApiUrl;
            RestClient = restClient;
        }

        public ActivationData Activate(string token)
        {
            var restRequest = new RestRequest(ResourceKey, Method.GET);
            restRequest.AddHeader("activation_token", token);

            var restResponse = RestClient.Execute<ActivationData>(restRequest);

            return restResponse.Data;
        }
    }
}