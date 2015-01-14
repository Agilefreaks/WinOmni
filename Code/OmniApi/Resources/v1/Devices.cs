namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using OmniApi.Models;
    using OmniCommon.Interfaces;
    using Refit;

    public class Devices : ResourceWithAuthorization<IDevicesApi>, IDevices
    {
        #region Constants

        public const string NotificationProvider = "omni_sync";

        #endregion

        #region Constructors and Destructors

        public Devices(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<EmptyModel> Update(object deviceParams)
        {
            return ResourceApi.Update(deviceParams, AccessToken);
        }

        public IObservable<Device> Activate(string registrationId, string identifier)
        {
            var device = new Device(identifier, registrationId) { Provider = NotificationProvider };

            return ResourceApi.Activate(device, AccessToken, ConfigurationService.Version.ToString());
        }

        public IObservable<EmptyModel> Call(string phoneNumber)
        {
            return ResourceApi.Call(phoneNumber, AccessToken);
        }

        public IObservable<Device> Create(string identifier, string name, string publicKey)
        {
            var device = new Device(identifier) { Name = name, PublicKey = publicKey };
            return ResourceApi.Create(device, AccessToken);
        }

        public IObservable<Device> Deactivate(string identifier)
        {
            var device = new Device(identifier);
            return ResourceApi.Deactivate(device, AccessToken);
        }

        public IObservable<EmptyModel> Remove(string identifier)
        {
            return ResourceApi.Remove(identifier, AccessToken);
        }

        public IObservable<EmptyModel> EndCall()
        {
            return ResourceApi.EndCall(AccessToken);
        }

        public IObservable<List<Device>> GetAll()
        {
            return ResourceApi.GetAll(AccessToken);
        }

        public IObservable<EmptyModel> SendSms(string phoneNumber, string content)
        {
            return ResourceApi.SendSms(phoneNumber, content, AccessToken);
        }

        #endregion

        #region Methods

        protected override IDevicesApi CreateResourceApi(HttpClient httpClient)
        {
            return RestService.For<IDevicesApi>(httpClient);
        }

        #endregion
    }
}