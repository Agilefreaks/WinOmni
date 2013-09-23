namespace Omnipaste.OmniClipboard.Infrastructure.Api.Resources
{
    using System.Net;
    using Omnipaste.OmniClipboard.Core.Api;
    using Omnipaste.OmniClipboard.Infrastructure.Services;
    using RestSharp;
    using Omnipaste.OmniClipboard.Core.Api.Models;
    using Omnipaste.OmniClipboard.Core.Api.Resources;
    using JsonSerializer = Omnipaste.OmniClipboard.Infrastructure.Api.JsonSerializer;

    public class Clippings : ApiResource, IClippings
    {
        public const string ResourceKey = "clippings";

        public string ApiKey { get; set; }

        public Clippings(IConfigurationManager configurationManager, IRestClient restClient)
            : base(configurationManager, restClient)
        {
        }

        public void SaveAsync(string data, ISaveClippingCompleteHandler handler)
        {
           var restRequest = new RestRequest(ResourceKey, Method.POST)
                                  {
                                      RequestFormat = DataFormat.Json,
                                      JsonSerializer = new JsonSerializer()
                                  };
            
            restRequest.AddBody(new Clipping(ApiKey, data));

            RestClient.ExecuteAsync<Clipping>(restRequest, (response, handle) => HandleSaveClippingCompleted(response, handler));
        }

        public void GetLastAsync(IGetClippingCompleteHandler handler)
        {
            var restRequest = new RestRequest(ResourceKey, Method.GET)
                                  {
                                      RequestFormat = DataFormat.Json
                                  };
            restRequest.AddHeader("Channel", ApiKey);

            RestClient.ExecuteAsync<Clipping>(restRequest, (response, handle) => HandleGetClippingCompleted(response, handler));
        }

        private void HandleSaveClippingCompleted(IRestResponse<Clipping> response, ISaveClippingCompleteHandler handler)
        {
            if (response.StatusCode == HttpStatusCode.Created)
            {
                handler.SaveClippingSucceeded();
            }
            else
            {
                handler.SaveClippingFailed(response.Content);
            }
        }

        private void HandleGetClippingCompleted(IRestResponse<Clipping> response, IGetClippingCompleteHandler handler)
        {
            if (response.StatusCode == HttpStatusCode.OK && response.Data != null)
            {
                handler.HandleClipping(response.Data.Content);
            }
        }
    }
}
