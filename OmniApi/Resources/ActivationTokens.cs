
namespace OmniApi.Resources
{
    using global::OmniApi.Models;
    using Omnipaste.OmniClipboard.Infrastructure.Api;
    using RestSharp;

    public class ActivationTokens : ApiResource, IActivationTokens
    {
        public readonly string ResourceKey = "activation_tokens";

        public ActivationModel Activate(string token)
        {
            var restRequest = new RestRequest(this.ResourceKey, Method.PUT)
                                  {
                                      RequestFormat = DataFormat.Json,
                                      JsonSerializer = new JsonSerializer()
                                  };
            restRequest.AddBody(new { token, device = "windows" });

            var restResponse = this.RestClient.Execute<ActivationModel>(restRequest);

            return restResponse.Data;
        }
    }
}