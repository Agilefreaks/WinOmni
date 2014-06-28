namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Models;

    public interface IDevices
    {
        Token Token { get; set; }

        IObservable<Device> Activate(string registrationId, string identifier);

        IObservable<Device> Create(string identifier, string name);

        IObservable<Device> Deactivate(string identifier);
    }
}