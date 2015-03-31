namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using OmniApi.Dto;
    using OmniCommon.Interfaces;
    using Refit;

    public class Devices : ResourceWithAuthorization<IDevicesApi>, IDevices
    {
        #region Constants

        public const string NotificationsProvider = "omni_sync";

        #endregion

        #region Constructors and Destructors

        public Devices(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<EmptyDto> Activate(string registrationId, string deviceId)
        {
            return Update(deviceId, new { RegistrationId = registrationId, Provider = NotificationsProvider });
        }

        public IObservable<DeviceDto> Create(string name, string publicKey)
        {
            var device = new DeviceDto { Name = name, PublicKey = publicKey };
            return ResourceApi.Create(device, AccessToken);
        }

        public IObservable<EmptyDto> Deactivate(string deviceId)
        {
            return Update(deviceId, new { RegistrationId = string.Empty });
        }

        public IObservable<List<DeviceDto>> GetAll()
        {
            return ResourceApi.GetAll(AccessToken);
        }

        public IObservable<DeviceDto> Get(string deviceId)
        {
            return ResourceApi.Get(deviceId, AccessToken);
        }

        public IObservable<EmptyDto> Remove(string deviceId)
        {
            return ResourceApi.Remove(deviceId, AccessToken);
        }

        public IObservable<EmptyDto> Update(string deviceId, object deviceParams)
        {
            return ResourceApi.Patch(deviceId, deviceParams, AccessToken, ConfigurationService.Version.ToString());
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