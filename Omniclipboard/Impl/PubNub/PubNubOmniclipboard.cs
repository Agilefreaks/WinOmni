using System;
using System.Collections.Generic;
using System.Linq;
using Omniclipboard.Services;

namespace Omniclipboard.Impl.PubNub
{
    public class PubNubOmniclipboard : IOmniclipboard
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

        public PubNubOmniclipboard(IConfigurationService configurationService, IPubNubClientFactory clientFactory)
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
                _pubnub.unsubscribe(_channel, o => { });
            }
        }

        public bool Initialize()
        {
            var communicationSettings = _configurationService.CommunicationSettings;
            var result = false;
            if (!string.IsNullOrEmpty(communicationSettings.Channel))
            {
                _channel = communicationSettings.Channel;
                _pubnub = _clientFactory.Create();
                _pubnub.subscribe(_channel, HandleMessageReceived);
                result = true;
            }

            return result;
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
