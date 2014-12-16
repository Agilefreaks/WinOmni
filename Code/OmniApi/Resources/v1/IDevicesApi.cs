namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using OmniApi.Models;
    using Refit;

    public interface IDevicesApi
    {
        #region Public Methods and Operators

        [Put("/devices")]
        IObservable<EmptyModel> Update([Body] object deviceParams);

        [Put("/devices/activate")]
        IObservable<Device> Activate([Body] Device device, [Header("Authorization")] string token, [Header("Client-Version")] string version);

        [Post("/devices")]
        IObservable<Device> Create([Body] Device device, [Header("Authorization")] string token);

        [Put("/devices/deactivate")]
        IObservable<Device> Deactivate([Body] Device device, [Header("Authorization")] string token);

        [Get("/devices")]
        IObservable<List<Device>> GetAll([Header("Authorization")] string token);

        [Post("/devices/end_call")]
        IObservable<EmptyModel> EndCall([Header("Authorization")] string token);

        [Post("/devices/sms")]
        IObservable<EmptyModel> SendSms([AliasAs("phone_number")] string phoneNumber, string content, [Header("Authorization")] string token);

        [Post("/devices/call")]
        IObservable<EmptyModel> Call([AliasAs("phone_number")] string phoneNumber, [Header("Authorization")] string token);

        #endregion
    }
}