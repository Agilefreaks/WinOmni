namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using OmniApi.Models;

    public interface IDevices
    {
        #region Public Methods and Operators

        IObservable<Device> Activate(string registrationId, string identifier);

        IObservable<Device> Create(string identifier, string name);

        IObservable<Device> Deactivate(string identifier);

        IObservable<List<Device>> GetAll();

        #endregion
    }
}