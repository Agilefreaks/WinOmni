namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using OmniApi.Models;
    using Refit;

    public interface IDevicesApi
    {
        #region Public Methods and Operators

        [Post("/user/devices")]
        IObservable<Device> Create([Body] Device device, [Header("Authorization")] string token);

        [Get("/user/devices")]
        IObservable<List<Device>> GetAll([Header("Authorization")] string token);

        [Delete("/user/devices/{id}")]
        IObservable<EmptyModel> Remove(string id, [Header("Authorization")] string accessToken);

        [Patch("/user/devices/{id}")]
        IObservable<EmptyModel> Patch(string id, [Body] object deviceParams, [Header("Authorization")] string token, [Header("Client-Version")] string version);

        #endregion
    }
}