namespace SMS.Resources.v1
{
    using System;
    using System.Net.Http;
    using OmniApi.Models;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using Refit;
    using SMS.Models;

    public class SMSMessages : ResourceWithAuthorization<ISMSMessagesApi>, ISMSMessages
    {
        #region Constructors and Destructors

        public SMSMessages(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<SmsMessage> Get(string id)
        {
            return ResourceApi.Get(id, AccessToken);
        }

        public IObservable<EmptyModel> Send(string phoneNumber, string message)
        {
            var payload = new SmsMessage
                              {
                                  Content = message,
                                  PhoneNumber = phoneNumber,
                                  Type = SmsMessageType.Outgoing,
                                  State = SmsMessageState.Sending
                              };

            return ResourceApi.Create(payload, AccessToken);
        }

        #endregion

        #region Methods

        protected override ISMSMessagesApi CreateResourceApi(HttpClient httpClient)
        {
            return RestService.For<ISMSMessagesApi>(httpClient);
        }

        #endregion
    }
}