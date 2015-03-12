namespace SMS.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
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

        public IObservable<SmsMessageDto> Get(string id)
        {
            return ResourceApi.Get(id, AccessToken);
        }

        public IObservable<SmsMessageDto> Send(string phoneNumber, string message)
        {
            var payload = new SmsMessageDto
                              {
                                  Content = message,
                                  PhoneNumber = phoneNumber,
                                  DeviceId = ConfigurationService.DeviceId,
                                  Type = SmsMessageType.Outgoing,
                                  State = SmsMessageState.Sending
                              };

            return ResourceApi.Create(payload, AccessToken);
        }

        public IObservable<SmsMessageDto> Send(IList<string> phoneNumbers, string message)
        {

            var payload =
                new 
                    {
                        Content = message,
                        PhoneNumberList = phoneNumbers.ToArray(),
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