namespace SMS.Resources.v1
{
    using System;
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

        public IObservable<EmptyModel> Send(string[] contentList, string[] phoneNumberList)
        {
            return ResourceApi.Create(new { ContentList = contentList, PhoneNumberList = phoneNumberList }, AccessToken);
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