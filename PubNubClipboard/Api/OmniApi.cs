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

        public void SaveClippingAsync(string data, ISaveClippingCompleteHandler saveClippingCompleteHandler)
        {
            var client = new RestClient(_apiConfig.BaseUrl);
            var restRequest = new RestRequest(_apiConfig.Resources.Clippings);
            restRequest.AddHeader("token", _communicationSettings.Channel);
            client.ExecuteAsyncPost(restRequest, (response, handle) => HandleSaveClippingCompleted(response, saveClippingCompleteHandler), "POST");
        }

        private static void HandleSaveClippingCompleted(IRestResponse response, ISaveClippingCompleteHandler saveClippingCompleteHandler)
        {
            if (response.StatusCode == HttpStatusCode.Created)
            {
                saveClippingCompleteHandler.SaveClippingSucceeded();
            }
            else
            {
                saveClippingCompleteHandler.SaveClippingFailed();
            }
        }
    }
}
