namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using OmniApi.Models;

    public interface IDevices
    {
        #region Public Methods and Operators

        IObservable<EmptyModel> Update(object deviceParams);

        IObservable<Device> Activate(string registrationId, string identifier);

        IObservable<EmptyModel> Call(string phoneNumber);

        IObservable<Device> Create(string identifier, string name);

        IObservable<Device> Deactivate(string identifier);

        IObservable<EmptyModel> EndCall();

        IObservable<List<Device>> GetAll();

        IObservable<EmptyModel> SendSms(string phoneNumber, string content);

        #endregion
    }
}