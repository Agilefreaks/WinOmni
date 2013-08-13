using System;
using System.Collections;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Omnipaste.OmniClipboard.Core.Messaging;

namespace Omnipaste.OmniClipboard.Infrastructure.Messaging
{
    public class PubNubMessagingService : IMessagingService
    {
        private readonly IPubNubClientFactory _clientFactory;
        private IPubNubClient _client;

        public bool IsInitialized { get; private set; }

        public bool IsInitializing { get; private set; }

        public IPubNubClient Client
        {
            get { return _client ?? (_client = _clientFactory.Create()); }
        }

        public PubNubMessagingService(IPubNubClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public bool Connect(string channel, IMessageHandler messageHandler)
        {
            var autoResetEvent = new AutoResetEvent(false);
            Action<string> statusCallback = result => HandleSubscribeConnectionStatusChanged(result, autoResetEvent);
            Client.Subscribe(channel, messageHandler.MessageReceived, statusCallback);
            autoResetEvent.WaitOne();

            return IsInitialized;
        }

        public void Disconnect(string channel)
        {
            if (!IsInitialized)
            {
                return;
            }

            Client.EndPendingRequests();
            Client.Unsubscribe(channel, o => { }, o => { }, o => { });
        }

        public void SendAsync(string channel, string message, IMessageHandler messageHandler)
        {
            Client.Publish(channel, message, o => PublishCallback(o, message, messageHandler));
        }

        private void PublishCallback(object response, string sentMessage, IMessageHandler handler)
        {
            var list = response as IList;
            if (list == null || list.Count <= 1) return;

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
                    IsInitialized = true;
                }
            }

            IsInitializing = false;
            autoResetEvent.Set();
        }

        private static string[] GetDataEntries(string receivedMessage)
        {
            var jsonSerializer = new JsonSerializer();
            var dataEntries = jsonSerializer.Deserialize(new StringReader(receivedMessage), typeof(string[])) as string[];

            return dataEntries;
        }
    }
}
