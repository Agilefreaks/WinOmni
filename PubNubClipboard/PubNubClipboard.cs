using System;
using System.Linq;
using OmniCommon;
using OmniCommon.Interfaces;

namespace PubNubClipboard
{
    using System.IO;

    using Newtonsoft.Json;

    using PubNubWrapper;

    public class PubNubClipboard : IPubNubClipboard
    {
        private readonly IConfigurationService _configurationService;
        private readonly IPubNubClientFactory _clientFactory;
        private IPubNubClient _pubnub;

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
        }

        public bool Initialize()
        {
            var communicationSettings = _configurationService.CommunicationSettings;
            if (!IsInitialized && !string.IsNullOrEmpty(communicationSettings.Channel))
            {
                Channel = communicationSettings.Channel;
                _pubnub = _clientFactory.Create();
                _pubnub.Subscribe<string>(Channel, HandleMessageReceived, o => { });
            }

            return IsInitialized;
        }

        public void Dispose()
        {
            if (!IsInitialized)
            {
                return;
            }

            _pubnub.EndPendingRequests();
            _pubnub.Unsubscribe(Channel, o => { }, o => { }, o => { });
        }

        public void SendData(string data)
        {
            _pubnub.Publish(Channel, data, o => { });
        }

        protected virtual void OnDataReceived(ClipboardEventArgs eventArgs)
        {
            if (DataReceived != null)
            {
                DataReceived(this, eventArgs);
            }
        }

        private void HandleMessageReceived(string receivedMessage)
        {
            var jsonSerializer = new JsonSerializer();
            var dataEntries = jsonSerializer.Deserialize(new StringReader(receivedMessage), typeof(string[])) as string[];
            if (dataEntries != null && dataEntries.Any())
            {
                OnDataReceived(new ClipboardEventArgs { Data = dataEntries[0] });
            }
        }
    }
}
