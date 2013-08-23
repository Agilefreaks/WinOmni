namespace Omnipaste.OmniClipboard.Infrastructure.Api.Resources
{
    using System.Net;
    using Omnipaste.OmniClipboard.Core.Api;
    using RestSharp;
    using Omnipaste.OmniClipboard.Core.Api.Models;
    using Omnipaste.OmniClipboard.Core.Api.Resources;

    public class Clippings : IClippings
    {
        public const string ResourceKey = "clippings";

        public string BaseUrl { get; set; }

        public string Version { get; set; }

        public string ApiKey { get; set; }

        public string ApiUrl
        {
            get { return string.Format("{0}/{1}", BaseUrl, Version); }
        }

        public Clippings(string baseUrl, string version, string apiKey)
        {
            BaseUrl = baseUrl;
            Version = version;
            ApiKey = apiKey;
        }

        public void SaveAsync(string data, ISaveClippingCompleteHandler handler)
        {
            var client = new RestClient(ApiUrl);

            var restRequest = new RestRequest(ResourceKey, Method.POST);
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.AddBody(new Clipping(ApiKey, data));

            client.ExecuteAsync<Clipping>(restRequest, (response, handle) => HandleSaveClippingCompleted(response, handler));
        }

        public void GetLastAsync(IGetClippingCompleteHandler handler)
        {
            var client = new RestClient(ApiUrl);

            var restRequest = new RestRequest(ResourceKey, Method.GET);
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.AddHeader("Channel", ApiKey);

            client.ExecuteAsync<Clipping>(restRequest, (response, handle) => HandleGetClippingCompleted(response, handler));
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
