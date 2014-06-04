namespace OmniSync
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using Ninject;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class OmniSyncService : IOmniSyncService
    {
        #region Fields
        private readonly IWebsocketConnectionFactory _websocketConnectionFactory;

        private IWebsocketConnection _websocketConnection;

        private ISubject<OmniMessage> _subject;

        #endregion

        #region Properties

        public ServiceStatusEnum Status { get; private set; }

        [Inject]
        public IEnumerable<IOmniMessageHandler> MessageHandlers { get; set; }

        #endregion

        public OmniSyncService(IWebsocketConnectionFactory websocketConnectionFactory)
        {
            _websocketConnectionFactory = websocketConnectionFactory;
        }

        public async Task<RegistrationResult> Start()
        {
            if (Status != ServiceStatusEnum.Stopped)
            {
                return new RegistrationResult { Data = _websocketConnection.RegistrationId };
            }

            _websocketConnection = await _websocketConnectionFactory.Create(ConfigurationManager.AppSettings["OmniSyncUrl"]);
            _subject = _websocketConnection.Connect();

            SubscribeMessageHandlers();
            
            Status = ServiceStatusEnum.Started;

            return new RegistrationResult { Data = _websocketConnection.RegistrationId };
        }

        public void Stop()
        {
            if (Status != ServiceStatusEnum.Started)
            {
                return;
            }

            UnsubscribeMessageHandlers();

            _websocketConnection.Disconnect();

            Status = ServiceStatusEnum.Stopped;
        }

        private void SubscribeMessageHandlers()
        {
            foreach (var messageHandler in MessageHandlers)
            {
                messageHandler.SubscribeTo(_subject);
            }
        }

        private void UnsubscribeMessageHandlers()
        {
            foreach (var messageHandler in MessageHandlers)
            {
                messageHandler.Dispose();
            }
        }
    }
}