namespace PubNubClipboard
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using OmniCommon;
    using OmniCommon.Interfaces;

    using PubNubWrapper;

    public class PubNubClipboard : IPubNubClipboard
    {
        private readonly IConfigurationService _configurationService;
        private readonly IPubNubClientFactory _clientFactory;
        private IPubNubClient _pubnub;
        private Task<bool> _initializationTask;

        public event EventHandler<ClipboardEventArgs> DataReceived;

        public string Channel { get; private set; }

        public bool IsInitialized { get; private set; }

        public bool IsInitializing { get; private set; }

        public PubNubClipboard(IConfigurationService configurationService, IPubNubClientFactory clientFactory)
        {
            _configurationService = configurationService;
            _clientFactory = clientFactory;
        }

        public Task<bool> Initialize()
        {
            var communicationSettings = _configurationService.CommunicationSettings;
            var task = string.IsNullOrEmpty(communicationSettings.Channel)
                           ? Task.Factory.StartNew(() => false)
                           : (IsInitializing ? _initializationTask : InitializePubNubClientAsync());

            return task;
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

        private static string[] GetDataEntries(string receivedMessage)
        {
            var jsonSerializer = new JsonSerializer();
            var dataEntries = jsonSerializer.Deserialize(new StringReader(receivedMessage), typeof(string[])) as string[];

            return dataEntries;
        }

        private Task<bool> InitializePubNubClientAsync()
        {
            return _initializationTask = Task<bool>.Factory.StartNew(InitializePubNubClient);
        }

        private bool InitializePubNubClient()
        {
            IsInitializing = true;
            _pubnub = _clientFactory.Create();
            Channel = _configurationService.CommunicationSettings.Channel;
            var autoResetEvent = new AutoResetEvent(false);
            Action<string> statusCallback = result => HandleSubscribeConnectionStatusChanged(result, autoResetEvent);
            _pubnub.Subscribe(Channel, HandleMessageReceived, statusCallback);
            autoResetEvent.WaitOne();

            return IsInitialized;
        }

        private void HandleSubscribeConnectionStatusChanged(string result, AutoResetEvent autoResetEvent)
        {
            var dataEntries = GetDataEntries(result);
            if (dataEntries != null && dataEntries.Any() && dataEntries[0] == "1")
            {
                IsInitialized = true;
            }

            IsInitializing = false;
            autoResetEvent.Set();
        }

        private void HandleMessageReceived(string receivedMessage)
        {
            var dataEntries = GetDataEntries(receivedMessage);
            if (dataEntries != null && dataEntries.Any())
            {
                OnDataReceived(new ClipboardEventArgs { Data = dataEntries[0] });
            }
        }
    }
}
