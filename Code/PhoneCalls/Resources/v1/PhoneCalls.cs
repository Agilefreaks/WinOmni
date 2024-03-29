﻿namespace PhoneCalls.Resources.v1
{
    using System;
    using System.Net.Http;
    using global::PhoneCalls.Dto;
    using OmniApi.Dto;
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

        public IObservable<PhoneCallDto> Get(string id)
        {
            return ResourceApi.Get(id, AccessToken);
        }

        public IObservable<PhoneCallDto> Call(string phoneNumber, int? contactId = null)
        {
            return
                ResourceApi.Create(
                    new PhoneCallDto
                        {
                            Number = phoneNumber,
                            State = PhoneCallState.Starting,
                            Type = PhoneCallType.Outgoing,
                            ContactId = contactId
                        },
                    AccessToken);
        }

        public IObservable<EmptyDto> EndCall(string callId)
        {
            return ResourceApi.Patch(callId, new { ConfigurationService.DeviceId, State = PhoneCallState.Ending, Type = PhoneCallType.Incoming }, AccessToken);
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