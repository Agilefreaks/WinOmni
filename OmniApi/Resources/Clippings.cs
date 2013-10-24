namespace OmniApi.Resources
{
    using System.Net;
    using global::OmniApi.Models;
    using Omnipaste.OmniClipboard.Core.Api;
    using Omnipaste.OmniClipboard.Infrastructure.Api;
    using RestSharp;

    public class Clippings : ApiResource, IClippings
    {
        public const string ResourceKey = "clippings";

        public string ApiKey { get; set; }

        public void SaveAsync(string data, ISaveClippingCompleteHandler handler)
        {
           var restRequest = new RestRequest(ResourceKey, Method.POST)
                                  {
                                      RequestFormat = DataFormat.Json,
                                      JsonSerializer = new JsonSerializer()
                                  };
            
            restRequest.AddBody(new Clipping(this.ApiKey, data));

            this.RestClient.ExecuteAsync<Clipping>(restRequest, (response, handle) => this.HandleSaveClippingCompleted(response, handler));
        }

        public void GetLastAsync(IGetClippingCompleteHandler handler)
        {
            var restRequest = new RestRequest(ResourceKey, Method.GET)
                                  {
                                      RequestFormat = DataFormat.Json
                                  };
            restRequest.AddHeader("Channel", this.ApiKey);

            this.RestClient.ExecuteAsync<Clipping>(restRequest, (response, handle) => this.HandleGetClippingCompleted(response, handler));
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
