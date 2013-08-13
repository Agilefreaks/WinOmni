using PubNubClipboard.Api;

namespace PubNubClipboard
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;
    using Common.Logging.Simple;
    using Newtonsoft.Json;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;
    using PubNubWrapper;

    public class PubNubClipboard : ClipboardBase, IPubNubClipboard, ISaveClippingCompleteHandler, IGetClippingCompleteHandler
    {
        private readonly IConfigurationService _configurationService;
        private readonly IOmniApi _omniApi;
        private readonly IPubNubClientFactory _clientFactory;
        private IPubNubClient _pubnub;
        private Task<bool> _initializationTask;
        private ILog _logger;

        public string Channel { get; private set; }

        public bool IsInitialized { get; private set; }

        public bool IsInitializing { get; private set; }

        public ILog Logger
        {
            get
            {
                return _logger ?? (_logger = new NoOpLogger());
            }

            set
            {
                _logger = value;
            }
        }

        public PubNubClipboard(IConfigurationService configurationService, IOmniApi omniApi, IPubNubClientFactory clientFactory)
        {
            _configurationService = configurationService;
            _omniApi = omniApi;
            _clientFactory = clientFactory;
        }

        public override Task<bool> Initialize()
        {
            var communicationSettings = _configurationService.CommunicationSettings;
            var task = string.IsNullOrEmpty(communicationSettings.Channel)
                           ? Task.Factory.StartNew(() => false)
                           : IsInitializing ? _initializationTask : InitializePubNubClientAsync();

            return task;
        }

        public override void Dispose()
        {
            if (!IsInitialized)
            {
                return;
            }

            _pubnub.EndPendingRequests();
            _pubnub.Unsubscribe(Channel, o => { }, o => { }, o => { });
        }

        public override void PutData(string data)
        {
            _omniApi.SaveClippingAsync(data, this);
        }

        private static string[] GetDataEntries(string receivedMessage)
        {
            var jsonSerializer = new JsonSerializer();
            var dataEntries = jsonSerializer.Deserialize(new StringReader(receivedMessage), typeof(string[])) as string[];

            return dataEntries;
        }

        private Task<bool> InitializePubNubClientAsync()
        {
            IsInitializing = true;
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

        private void PutDataCallback(object obj)
        {
            var list = obj as IList;
            if (list == null || list.Count <= 1) return;

            var message = list[1].ToString();
            if (message.ToLowerInvariant() != "sent")
            {
                LogCallbackMessage(message);
            }
        }

        private void HandleSubscribeConnectionStatusChanged(string result, AutoResetEvent autoResetEvent)
        {
            var dataEntries = GetDataEntries(result);
            if (dataEntries != null && dataEntries.Length > 1)
            {
                LogCallbackMessage(dataEntries[1]);
                if (dataEntries[0] == "1")
                {
                    IsInitialized = true;
                }
            }

            IsInitializing = false;
            autoResetEvent.Set();
        }

        private void HandleMessageReceived(string receivedMessage)
        {
            _omniApi.GetLastClippingAsync(this);
        }

        private void LogCallbackMessage(string message)
        {
            Logger.Info(message);
        }

        public void SaveClippingSucceeded()
        {
            _pubnub.Publish(Channel, "NewMessage", PutDataCallback);
        }

        public void SaveClippingFailed()
        {
        }

        public void HandleClipping(string clip)
        {
            OnDataReceived(new ClipboardData(this, clip));
        }
    }
}
