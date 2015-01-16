namespace PhoneCalls.Resources.v1
{
    using System;
    using global::PhoneCalls.Models;
    using OmniApi.Models;
    using Refit;

    public interface IPhoneCallsApi
    {
        [Patch("/phone_calls/{id}")]
        IObservable<PhoneCall> Get(string id, [Header("Authorization")] string token);

        [Post("/phone_calls")]
        IObservable<EmptyModel> Create([Body] PhoneCall payload, [Header("Authorization")] string token);

        [Patch("/phone_calls/{id}")]
        IObservable<EmptyModel> Patch(string id, [Body] object payload, [Header("Authorization")] string token);
    }
}