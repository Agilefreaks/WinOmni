namespace PubNubClipboard.Api
{
    using System.Collections.Generic;
    using System.Net;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;
    using RestSharp;

    public class OmniApi : IOmniApi
    {
        private readonly IList<ISaveClippingCompleteHandler> _saveClippingListeners;

        private readonly ApiConfig _apiConfig;
        private readonly CommunicationSettings _communicationSettings;

        public OmniApi(IConfigurationService configurationService)
        {
            _saveClippingListeners = new List<ISaveClippingCompleteHandler>();
            _apiConfig = configurationService.ApiConfig;
            _communicationSettings = configurationService.CommunicationSettings;
        }

        public void AddSaveClippingCompleteListener(ISaveClippingCompleteHandler handler)
        {
            _saveClippingListeners.Add(handler);
        }

        public void RemoveSaveClippingCompleteListener(ISaveClippingCompleteHandler handler)
        {
            _saveClippingListeners.Remove(handler);
        }

        public void SaveClippingAsync(string data, ISaveClippingCompleteHandler handler)
        {
            var client = new RestClient(_apiConfig.BaseUrl);
            var restRequest = new RestRequest(_apiConfig.Resources.Clippings);
            restRequest.AddHeader("token", _communicationSettings.Channel);
            client.ExecuteAsyncPost(restRequest, (response, handle) => HandleSaveClippingCompleted(response, handler), "POST");
        }

        public void GetLastClippingAsync(IGetClippingCompleteHandler handler)
        {
            var client = new RestClient(_apiConfig.BaseUrl);
            var restRequest = new RestRequest(_apiConfig.Resources.Clippings);
            restRequest.AddHeader("token", _communicationSettings.Channel);
            client.ExecuteAsyncGet(restRequest, (response, handle) => HandleGetClippingCompleted(response, handler), "GET");
        }

        private void HandleGetClippingCompleted(IRestResponse response, IGetClippingCompleteHandler handler)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                handler.HandleClipping(response.Content);
            }
        }

        private void HandleSaveClippingCompleted(IRestResponse response, ISaveClippingCompleteHandler handler)
        {
            if (response.StatusCode == HttpStatusCode.Created)
            {
                handler.SaveClippingSucceeded();
            }
            else
            {
                handler.SaveClippingFailed();
            }
        }
    }
}
