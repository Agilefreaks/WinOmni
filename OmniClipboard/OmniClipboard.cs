namespace OmniClipboard
{   
    using System;
    using System.Threading.Tasks;
    using OmniApi;
    using OmniApi.Resources;
    using global::OmniClipboard.Messaging;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;
    using Omnipaste.OmniClipboard.Core.Api;

    public class OmniClipboard : ClipboardBase, IOmniClipboard, ISaveClippingCompleteHandler, IGetClippingCompleteHandler, IMessageHandler
    {
        private readonly IConfigurationService _configurationService;
        private readonly IMessagingService _messagingService;
        private Task<bool> _initializationTask;

        public string MessageGuid { get; set; }

        public string Channel { get; private set; }

        public IClippings Clippings { get; set; }

        public OmniClipboard(IConfigurationService configurationService, IMessagingService messagingService)
        {
            _configurationService = configurationService;
            _messagingService = messagingService;

            Clippings = OmniApi.Clippings;
        }

        public override Task<bool> Initialize()
        {
            var communicationSettings = _configurationService.CommunicationSettings;
            var task = string.IsNullOrEmpty(communicationSettings.Channel)
                           ? Task.Factory.StartNew(() => false)
                           : _initializationTask ?? (_initializationTask = Task<bool>.Factory.StartNew(DoInitialize));

            return task;
        }

        public bool DoInitialize()
        {
            Channel = _configurationService.CommunicationSettings.Channel;
            Clippings.Channel = Channel;

            return _messagingService.Connect(Channel, this);
        }

        public override void PutData(string data)
        {
            Clippings.SaveAsync(data, this);
        }

        public override void Dispose()
        {
            _messagingService.Disconnect(Channel);
        }

        void ISaveClippingCompleteHandler.SaveClippingSucceeded()
        {
            MessageGuid = Guid.NewGuid().ToString();
            _messagingService.SendAsync(Channel, MessageGuid, this);
        }

        void ISaveClippingCompleteHandler.SaveClippingFailed(string reason)
        {
            // TODO: use bugfreak core to log
        }

        void IMessageHandler.MessageReceived(string message)
        {
            if (MessageGuid == null || !message.Contains(MessageGuid))
            {
                Clippings.GetLastAsync(this);
            }
        }

        void IGetClippingCompleteHandler.HandleClipping(string clip)
        {
            NotifyReceivers(new ClipboardData(this, clip));
        }

        void IMessageHandler.MessageSent(string message)
        {
        }

        void IMessageHandler.MessageSendFailed(string message, string reason)
        {
            // TODO: use bugfreak core to log
        }
    }
}
