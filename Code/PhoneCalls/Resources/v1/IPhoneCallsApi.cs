namespace PhoneCalls.Resources.v1
{
    using System;
    using global::PhoneCalls.Models;
    using OmniApi.Models;
    using Refit;

    public interface IPhoneCallsApi
    {
        [Post("/phone_calls")]
        IObservable<EmptyModel> Create([Body] PhoneCall payload, [Header("Authorization")] string token);
    }
}