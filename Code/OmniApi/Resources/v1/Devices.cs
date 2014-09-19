namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using Ninject;
    using OmniApi.Models;
    using OmniCommon.Interfaces;
    using Refit;

    public class Devices : Resource<Devices.IDevicesApi>, IDevices
    {
        #region Constants

        public const string NotificationProvider = "omni_sync";

        #endregion

        #region Interfaces

        [ColdObservable]
        public interface IDevicesApi
        {
            #region Public Methods and Operators

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

        #endregion

        [Inject]
        public IApplicationService ApplicationService { get; set; }

        #region Public Methods and Operators

        public IObservable<Device> Activate(string registrationId, string identifier)
        {
            var device = new Device(identifier, registrationId) { Provider = NotificationProvider };

            return Authorize(ResourceApi.Activate(device, AccessToken, ApplicationService.Version.ToString()));
        }

        public IObservable<Device> Create(string identifier, string name)
        {
            var device = new Device(identifier) { Name = name };
            return Authorize(ResourceApi.Create(device, AccessToken));
        }

        public IObservable<Device> Deactivate(string identifier)
        {
            var device = new Device(identifier);
            return Authorize(ResourceApi.Deactivate(device, AccessToken));
        }

        public IObservable<List<Device>> GetAll()
        {
            var observable = ResourceApi.GetAll(AccessToken);
            return Authorize(observable);
        }

        public IObservable<EmptyModel> EndCall()
        {
            return Authorize(ResourceApi.EndCall(AccessToken));
        }

        public IObservable<EmptyModel> SendSms(string phoneNumber, string content)
        {
            return Authorize(ResourceApi.SendSms(phoneNumber, content, AccessToken));
        }

        public IObservable<EmptyModel> Call(string phoneNumber)
        {
            return Authorize(ResourceApi.Call(phoneNumber, AccessToken));
        }

        #endregion
    }
}