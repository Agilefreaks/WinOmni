using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Ninject;
using OmniCommon.DataProviders;
using OmniCommon.Interfaces;
using OmniCommon.Models;
using WampSharp;
using WampSharp.Auxiliary.Client;

namespace OmniSync
{

    public class NotificationService : INotificationService
    {
        #region Fields

        private readonly IConfigurationProvider _configurationProvider;

        private readonly IWampChannelFactory<JToken> _wampChannelFactory;

        private IWampChannel<JToken> _channel;

        private ISubject<OmniMessage> _subject;

        #endregion

        #region Properties

        public ServiceStatusEnum Status { get; set; }

        [Inject]
        public IEnumerable<IOmniMessageHandler> MessageHandlers { get; set; }

        #endregion

        public NotificationService(IConfigurationProvider configurationProvider, IWampChannelFactory<JToken> wampChannelFactory)
        {
            _configurationProvider = configurationProvider;
            _wampChannelFactory = wampChannelFactory;
        }

        public async Task<RegistrationResult> Start()
        {
            if (Status != ServiceStatusEnum.Stopped)
            {
                return new RegistrationResult { Error = new Exception("Notification service is already started.") };
            }

            var registrationResult = await ConnectToServer();
            OpenWebsocket();
            Status = ServiceStatusEnum.Started;

            foreach (var messageHandler in MessageHandlers)
            {
                messageHandler.SubscribeTo(this);
            }

            return registrationResult;
        }

        private void OpenWebsocket()
        {
            //TODO: channel (which are the email address) should be encoded to be valid on URL
            _subject = _channel.GetSubject<OmniMessage>(_configurationProvider["channel"]);
        }

        private async Task<RegistrationResult> ConnectToServer()
        {
            _channel = _wampChannelFactory.CreateChannel(ConfigurationManager.AppSettings["OmniSyncUrl"]);
            await _channel.OpenAsync();
            var wampClientConnectionMonitor = (WampClientConnectionMonitor<JToken>)_channel.GetMonitor();
            var registrationResult = new RegistrationResult { Data = wampClientConnectionMonitor.SessionId };

            return registrationResult;
        }

        public void Stop()
        {
            if (Status == ServiceStatusEnum.Started)
            {
                foreach (var messageHandler in MessageHandlers)
                {
                    messageHandler.Dispose();
                }

                _channel.Close();
            }
        }

        public IDisposable Subscribe(IObserver<OmniMessage> observer)
        {
            if (Status != ServiceStatusEnum.Started)
            {
                //TODO: throw meaningful exception
                throw new Exception();
            }

            return _subject.Subscribe(observer);
        }
    }
}