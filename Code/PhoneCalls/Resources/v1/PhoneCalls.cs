namespace PhoneCalls.Resources.v1
{
    using System;
    using System.Net.Http;
    using global::PhoneCalls.Models;
    using OmniApi.Models;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using Refit;

    public class PhoneCalls : ResourceWithAuthorization<IPhoneCallsApi>, IPhoneCalls
    {
        #region Constructors and Destructors

        public PhoneCalls(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<EmptyModel> Call(string phoneNumber)
        {
            return
                ResourceApi.Create(
                    new PhoneCall
                        {
                            Number = phoneNumber,
                            State = PhoneCallState.Starting,
                            Type = PhoneCallType.Outgoing
                        },
                    AccessToken);
        }

        public IObservable<EmptyModel> EndCall(string callId)
        {
            return ResourceApi.Patch(callId, new  { State = PhoneCallState.Ending }, AccessToken);
        }

        #endregion

        #region Methods

        protected override IPhoneCallsApi CreateResourceApi(HttpClient httpClient)
        {
            return RestService.For<IPhoneCallsApi>(httpClient);
        }

        #endregion
    }
}