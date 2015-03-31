namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using OmniApi.Dto;

    public interface IDevices
    {
        #region Public Methods and Operators

        IObservable<EmptyDto> Update(string deviceId, object deviceParams);

        IObservable<EmptyDto> Activate(string registrationId, string deviceId);

        IObservable<DeviceDto> Create(string name, string publicKey);

        IObservable<EmptyDto> Deactivate(string deviceId);

        IObservable<EmptyDto> Remove(string deviceId);

        IObservable<List<DeviceDto>> GetAll();

        IObservable<DeviceDto> Get(string deviceId);

        #endregion
    }
}