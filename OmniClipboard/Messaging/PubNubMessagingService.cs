namespace OmniClipboard.Messaging
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Threading;
    using Newtonsoft.Json;

    public class PubNubMessagingService : IMessagingService
    {
        private readonly IPubNubClientFactory _clientFactory;
        private IPubNubClient _client;

        public bool IsInitialized { get; private set; }

        public bool IsInitializing { get; private set; }

        public IPubNubClient Client
        {
            get { return this._client ?? (this._client = this._clientFactory.Create()); }
        }

        public PubNubMessagingService(IPubNubClientFactory clientFactory)
        {
            this._clientFactory = clientFactory;
        }

        public bool Connect(string channel, IMessageHandler messageHandler)
        {
            var autoResetEvent = new AutoResetEvent(false);
            Action<string> statusCallback = result => this.HandleSubscribeConnectionStatusChanged(result, autoResetEvent);
            Action<string> errorCallback = result => { };
            this.Client.Subscribe(channel, messageHandler.MessageReceived, statusCallback, errorCallback);
            autoResetEvent.WaitOne();

            return this.IsInitialized;
        }

        public void Disconnect(string channel)
        {
            if (!this.IsInitialized)
            {
                return;
            }

            this.Client.EndPendingRequests();
            this.Client.Unsubscribe<string>(channel, o => { }, o => { }, o => { }, o => { });
        }

        public void SendAsync(string channel, string message, IMessageHandler messageHandler)
        {
            this.Client.Publish<string>(channel, message, o => this.PublishCallback(o, message, messageHandler), o => { });
        }

        private static string[] GetDataEntries(string receivedMessage)
        {
            var jsonSerializer = new JsonSerializer();
            var dataEntries = jsonSerializer.Deserialize(new StringReader(receivedMessage), typeof(string[])) as string[];

            return dataEntries;
        }

        private void PublishCallback(object response, string sentMessage, IMessageHandler handler)
        {
            var list = response as IList;
            if (list == null || list.Count <= 1)
            {
                return;
            }

            var responseMessage = list[1].ToString();
            if (responseMessage.ToLowerInvariant() != "sent")
            {
                handler.MessageSendFailed(sentMessage, responseMessage);
            }
        }

        private void HandleSubscribeConnectionStatusChanged(string result, AutoResetEvent autoResetEvent)
        {
            var dataEntries = GetDataEntries(result);
            if (dataEntries != null && dataEntries.Length > 1)
            {
                if (dataEntries[0] == "1")
                {
                    this.IsInitialized = true;
                }
            }

            this.IsInitializing = false;
            autoResetEvent.Set();
        }
    }
}
