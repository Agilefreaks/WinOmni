using System.Threading.Tasks;
using Common.Logging;
using Common.Logging.Simple;
using OmniCommon.Interfaces;
using OmniCommon.Services;
using Omnipaste.OmniClipboard.Core.Api;
using Omnipaste.OmniClipboard.Core.Messaging;

namespace Omnipaste.OmniClipboard.Core
{
    public class OmniClipboard : ClipboardBase, IOmniClipboard, ISaveClippingCompleteHandler, IGetClippingCompleteHandler, IMessageHandler
    {
        private readonly IConfigurationService _configurationService;
        private readonly IOmniApi _omniApi;
        private readonly IMessagingService _messagingService;
        private Task<bool> _initializationTask;
        private ILog _logger;

        public string Channel { get; private set; }

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

        public OmniClipboard(IConfigurationService configurationService, IOmniApi omniApi, IMessagingService messagingService)
        {
            _configurationService = configurationService;
            _omniApi = omniApi;
            _messagingService = messagingService;
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
            var connected = _messagingService.Connect(Channel, this);

            return connected;
        }

        public override void PutData(string data)
        {
            _omniApi.SaveClippingAsync(data, this);
        }

        public override void Dispose()
        {
            _messagingService.Disconnect(Channel);
        }

        void ISaveClippingCompleteHandler.SaveClippingSucceeded()
        {
            _messagingService.SendAsync(Channel, "NewMessage", this);
        }

        void ISaveClippingCompleteHandler.SaveClippingFailed(string reason)
        {
            Logger.Info(string.Format("Save failed because {0}", reason));
        }

        void IMessageHandler.MessageReceived(string message)
        {
            _omniApi.GetLastClippingAsync(this);
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
            Logger.Info(message);
        }
    }
}
