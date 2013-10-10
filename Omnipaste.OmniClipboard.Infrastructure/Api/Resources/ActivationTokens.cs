using OmniCommon.DataProviders;
using Omnipaste.OmniClipboard.Infrastructure.Services;
using RestSharp;

namespace Omnipaste.OmniClipboard.Infrastructure.Api.Resources
{
    using System;
    using System.Linq;
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
            var restRequest = new RestRequest(ResourceKey, Method.PUT)
                                  {
                                      RequestFormat = DataFormat.Json,
                                      JsonSerializer = new JsonSerializer()
                                  };
            restRequest.AddBody(new { token, device = "windows" });

            var restResponse = this.RestClient.Execute<ActivationData>(restRequest);

            return restResponse.Data;
        }
    }
}