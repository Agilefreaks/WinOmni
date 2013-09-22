namespace Omnipaste.OmniClipboard.Infrastructure.Api.Resources
{
    using RestSharp;

    public class Users
    {
        private string _apiUrl;

        public readonly string ResourceKey = "users";

        public IRestClient RestClient { get; set; }

        public string ApiUrl
        {
            get
            {
                return _apiUrl;
            }
            set
            {
                _apiUrl = value;
                RestClient.BaseUrl = value;
            }
        }

        public Users(IRestClient restClient)
        {
            RestClient = restClient;
        }

        public void Activate(string token)
        {
            var restRequest = new RestRequest(ResourceKey, Method.GET);
            restRequest.AddHeader("activation_token", token);

            RestClient.ExecuteAsync(restRequest, (response, handle) => { });
        }
    }
}