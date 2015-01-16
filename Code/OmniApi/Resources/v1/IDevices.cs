namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using OmniApi.Models;

    public interface IDevices
    {
        #region Public Methods and Operators

        IObservable<EmptyModel> Update(string deviceId, object deviceParams);

        IObservable<EmptyModel> Activate(string registrationId, string deviceId);

        IObservable<Device> Create(string name, string publicKey);

        IObservable<EmptyModel> Deactivate(string deviceId);

        IObservable<EmptyModel> Remove(string deviceId);

        IObservable<List<Device>> GetAll();

        #endregion
    }
}