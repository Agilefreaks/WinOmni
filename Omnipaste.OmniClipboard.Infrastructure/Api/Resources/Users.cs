namespace Omnipaste.OmniClipboard.Infrastructure.Api.Resources
{
    using OmniCommon.DataProviders;
    using Omnipaste.OmniClipboard.Core.Api.Resources;
    using Omnipaste.OmniClipboard.Infrastructure.Services;
    using RestSharp;

    public class Users : ApiResource, IUsers
    {
        public readonly string ResourceKey = "users";

        public readonly string ActivateMethodName = "activate";

        public Users(IConfigurationManager configuration, IRestClient restClient)
            : base(configuration, restClient)
        {
        }

        public ActivationData Activate(string token)
        {
            var restRequest = new RestRequest(string.Concat(ResourceKey, "/", ActivateMethodName), Method.GET);
            restRequest.AddHeader("Token", token);

            var restResponse = this.RestClient.Execute<ActivationData>(restRequest);

            return restResponse.Data;
        }
    }
}