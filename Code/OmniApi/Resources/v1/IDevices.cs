namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using OmniApi.Models;

    public interface IDevices
    {
        #region Public Methods and Operators

        IObservable<EmptyModel> Update(object deviceParams);

        IObservable<EmptyModel> Activate(string registrationId, string deviceId);

        IObservable<EmptyModel> Call(string phoneNumber);

        IObservable<Device> Create(string name, string publicKey);

        IObservable<EmptyModel> Deactivate(string identifier);

        IObservable<EmptyModel> Remove(string identifier);

        IObservable<EmptyModel> EndCall();

        IObservable<List<Device>> GetAll();

        IObservable<EmptyModel> SendSms(string phoneNumber, string content);

        #endregion
    }
}