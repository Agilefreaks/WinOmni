using OmniCommon.DataProviders;
using Omnipaste.OmniClipboard.Infrastructure.Services;
using RestSharp;

namespace Omnipaste.OmniClipboard.Infrastructure.Api.Resources
{
    using Omnipaste.OmniClipboard.Core.Api.Resources;

    public class ActivationTokens : ApiResource, IActivationTokens
    {
        public readonly string ResourceKey = "activation_tokens";

        public ActivationTokens(IConfigurationManager configurationManager, IRestClient restClient)
            : base(configurationManager, restClient)
        {
        }

        public ActivationData Activate(string token)
        {
            var restRequest = new RestRequest(ResourceKey, Method.PUT);
            restRequest.AddParameter("token", token, ParameterType.RequestBody);
            restRequest.AddParameter("device", "windows", ParameterType.RequestBody);

            var restResponse = this.RestClient.Execute<ActivationData>(restRequest);

            return restResponse.Data;
        }
    }
}