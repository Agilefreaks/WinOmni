﻿namespace PhoneCalls.Resources.v1
{
    using System;
    using global::PhoneCalls.Dto;
    using OmniApi.Dto;
    using Refit;

    public interface IPhoneCallsApi
    {
        [Get("/phone_calls/{id}")]
        IObservable<PhoneCallDto> Get(string id, [Header("Authorization")] string token);

        [Post("/phone_calls")]
        IObservable<PhoneCallDto> Create([Body] PhoneCallDto payload, [Header("Authorization")] string token);

        [Patch("/phone_calls/{id}")]
        IObservable<EmptyDto> Patch(string id, [Body] object payload, [Header("Authorization")] string token);
    }
}