namespace Omnipaste.OmniClipboard.Infrastructure.Api
{
    using System.Net;
    using OmniCommon.Interfaces;
    using Omnipaste.OmniClipboard.Core.Api;
    using RestSharp;

    public class OmniApi : IOmniApi
    {
        private const string ApiUrl = "http://localhost:3000/api/v1";

        private readonly IConfigurationService _configurationService;

        public OmniApi(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public void SaveClippingAsync(string data, ISaveClippingCompleteHandler handler)
        {
            var client = new RestClient(ApiUrl);

            var restRequest = new RestRequest(Clipping.ResourceKey, Method.POST);
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.AddBody(new Clipping(_configurationService.CommunicationSettings.Channel, data));

            client.ExecuteAsync<Clipping>(restRequest, (response, handle) => HandleSaveClippingCompleted(response, handler));
        }

        public void GetLastClippingAsync(IGetClippingCompleteHandler handler)
        {
            var client = new RestClient(ApiUrl);

            var restRequest = new RestRequest(string.Format("{0}/{1}/last", Clipping.ResourceKey, _configurationService.CommunicationSettings.Channel), Method.GET);
            restRequest.RequestFormat = DataFormat.Json;

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
