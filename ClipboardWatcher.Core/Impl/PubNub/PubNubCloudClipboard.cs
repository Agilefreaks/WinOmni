using System;
using System.Collections.Generic;
using System.Linq;
using ClipboardWatcher.Core.Services;

namespace ClipboardWatcher.Core.Impl.PubNub
{
    public class PubNubCloudClipboard : IPubNubCloudClipboard
    {
        private readonly IConfigurationService _configurationService;
        private readonly IPubNubClientFactory _clientFactory;
        private string _channel;
        private Pubnub _pubnub;

        public event EventHandler<ClipboardEventArgs> DataReceived;

        public bool IsInitialized
        {
            get { return _pubnub != null; }
        }

        public PubNubCloudClipboard(IConfigurationService configurationService, IPubNubClientFactory clientFactory)
        {
            _configurationService = configurationService;
            _clientFactory = clientFactory;
            Initialize();
        }

        public void Copy(string str)
        {
            _pubnub.publish(_channel, str, o => { });
        }

        public void Dispose()
        {
            if (IsInitialized)
            {
                _pubnub.EndPendingRequests();
            }
        }

        public void Initialize()
        {
            var communicationSettings = _configurationService.CommunicationSettings;
            if (string.IsNullOrEmpty(communicationSettings.Channel)) return;
            _channel = communicationSettings.Channel;
            _pubnub = _clientFactory.Create();
            _pubnub.subscribe(_channel, HandleMessageReceived);
        }

        protected virtual void OnDataReceived(ClipboardEventArgs e)
        {
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        private void HandleMessageReceived(object receivedMessage)
        {
            var dataOject = ((IEnumerable<object>)receivedMessage).First();
            var value = ((Newtonsoft.Json.Linq.JValue)dataOject).Value;

            OnDataReceived(new ClipboardEventArgs { Data = value.ToString() });
        }
    }
}
