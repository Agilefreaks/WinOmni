using System.Collections.Generic;
using System.Net;
using OmniCommon.Interfaces;
using OmniCommon.Services;
using Omnipaste.OmniClipboard.Core.Api;
using RestSharp;

namespace Omnipaste.OmniClipboard.Infrastructure.Api
{
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
            
            var restRequest = new RestRequest(_apiConfig.Resources.Clippings, Method.POST);
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.AddBody(new Clipping(_communicationSettings.Channel, data));

            client.ExecuteAsync<Clipping>(restRequest, (response, handle) => HandleSaveClippingCompleted(response, handler));
        }

        public void GetLastClippingAsync(IGetClippingCompleteHandler handler)
        {
            var client = new RestClient(_apiConfig.BaseUrl);
            
            var restRequest = new RestRequest(string.Format("{0}/{1}/last", _apiConfig.Resources.Clippings, _communicationSettings.Channel), Method.GET);
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
