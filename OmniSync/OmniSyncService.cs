namespace OmniSync
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using Castle.Core.Internal;
    using Newtonsoft.Json.Linq;
    using OmniCommon.Interfaces;
    using WampSharp;
    using WampSharp.Auxiliary.Client;

    public class OmniSyncService : IOmniSyncService
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly IWampChannelFactory<JToken> _wampChannelFactory;

        private IWampChannel<JToken> _channel;

        private ISubject<OmniMessage> _subject;

        private readonly IList<IDisposable> _subscriptions;

        #endregion

        #region Properties

        public ServiceStatusEnum Status { get; set; }

        #endregion

        public OmniSyncService(IConfigurationService configurationService, IWampChannelFactory<JToken> wampChannelFactory)
        {
            _configurationService = configurationService;
            _wampChannelFactory = wampChannelFactory;
            _subscriptions = new List<IDisposable>();
        }

        public async Task<RegistrationResult> Start()
        {
            if (Status != ServiceStatusEnum.Stopped)
            {
                return new RegistrationResult { Error = new Exception("OmniSync service is already started.") };
            }

            _channel = _wampChannelFactory.CreateChannel(_configurationService["OmniSyncUrl"]);
            await _channel.OpenAsync();

            var wampClientConnectionMonitor = (WampClientConnectionMonitor<JToken>)_channel.GetMonitor();
            var registrationResult = new RegistrationResult { Data = wampClientConnectionMonitor.SessionId };

            _subject = _channel.GetSubject<OmniMessage>(_configurationService.GetCommunicationChannel());
            Status = ServiceStatusEnum.Started;

            return registrationResult;
        }

        public void Stop()
        {
            if (Status == ServiceStatusEnum.Started)
            {
                _subscriptions.ForEach(x => x.Dispose());

                _channel.Close();
            }
        }

        public IDisposable Subscribe(IObserver<OmniMessage> observer)
        {
            if (Status != ServiceStatusEnum.Started)
            {
                throw new Exception();
            }

            var subscription = _subject.Subscribe(observer);
            _subscriptions.Add(subscription);

            return subscription;
        }

        public bool Unsubscribe(IDisposable subscription)
        {
            var result = _subscriptions.Remove(subscription);
            subscription.Dispose();

            return result;
        }

        public RegistrationResult GetRegistrationId(string communicationChannel)
        {
            return new RegistrationResult();
        }
    }
}