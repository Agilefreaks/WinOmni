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

        public IObservable<EmptyModel> Send(string message, string phoneNumber)
        {
            var payload =
                new
                    {
                        Content = message,
                        PhoneNumber = phoneNumber,
                        Type = SMSMessageType.Incoming,
                        State = SMSMessageState.Sending
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