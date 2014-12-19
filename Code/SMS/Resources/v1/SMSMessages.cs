namespace SMS.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using OmniApi.Models;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using Refit;

    public class SMSMessages : ResourceWithAuthorization<ISMSMessagesApi>, ISMSMessages
    {
        #region Constructors and Destructors

        public SMSMessages(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<EmptyModel> Send(IEnumerable<string> messages, IEnumerable<string> phoneNumbers)
        {
            var payload = new { ContentList = messages.ToArray(), PhoneNumberList = phoneNumbers.ToArray() };

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