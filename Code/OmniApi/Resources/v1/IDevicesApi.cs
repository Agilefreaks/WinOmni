namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using OmniApi.Dto;
    using Refit;

    public interface IDevicesApi
    {
        #region Public Methods and Operators

        [Post("/user/devices")]
        IObservable<DeviceDto> Create([Body] DeviceDto deviceDto, [Header("Authorization")] string token);

        [Get("/user/devices/{id}")]
        IObservable<DeviceDto> Get([AliasAs("id")] string id, [Header("Authorization")] string token);

        [Get("/user/devices")]
        IObservable<List<DeviceDto>> GetAll([Header("Authorization")] string token);

        [Delete("/user/devices/{id}")]
        IObservable<EmptyDto> Remove(string id, [Header("Authorization")] string accessToken);

        [Patch("/user/devices/{id}")]
        IObservable<EmptyDto> Patch(string id, [Body] object deviceParams, [Header("Authorization")] string token, [Header("Client-Version")] string version);

        #endregion
    }
}