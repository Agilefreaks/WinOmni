using System;
using System.Collections.Generic;
using System.Linq;
using OmniCommon;
using OmniCommon.Interfaces;

namespace PubNubClipboard.Impl.PubNub
{
    public class PubNubClipboard : IPubNubClipboard
    {
        private readonly IConfigurationService _configurationService;
        private readonly IPubNubClientFactory _clientFactory;
        private Pubnub _pubnub;

        public string Channel { get; private set; }

        public event EventHandler<ClipboardEventArgs> DataReceived;

        public bool IsInitialized
        {
            get { return _pubnub != null; }
        }

        public PubNubClipboard(IConfigurationService configurationService, IPubNubClientFactory clientFactory)
        {
            _configurationService = configurationService;
            _clientFactory = clientFactory;
            Initialize();
        }

        public bool Initialize()
        {
            var communicationSettings = _configurationService.CommunicationSettings;
            var result = false;
            if (!string.IsNullOrEmpty(communicationSettings.Channel))
            {
                Channel = communicationSettings.Channel;
                _pubnub = _clientFactory.Create();
                _pubnub.subscribe(Channel, HandleMessageReceived);
                result = true;
            }

            return result;
        }

        public void Dispose()
        {
            if (!IsInitialized)
            {
                return;
            }

            _pubnub.EndPendingRequests();
            _pubnub.unsubscribe(Channel, o => { });
        }

        public void SendData(string data)
        {
            _pubnub.publish(Channel, data, o => { });
        }

        protected virtual void OnDataReceived(ClipboardEventArgs eventArgs)
        {
            if (DataReceived != null)
            {
                DataReceived(this, eventArgs);
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
