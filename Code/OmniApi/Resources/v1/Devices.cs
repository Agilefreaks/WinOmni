namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Ninject;
    using OmniApi.Models;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Refit;

    public class Devices : Resource<IDevicesApi>, IDevices
    {
        #region Constants

        public const string NotificationProvider = "omni_sync";

        #endregion

        #region Constructors and Destructors

        public Devices()
            : base(CreateResourceApi())
        {
        }

        #endregion

        #region Public Properties

        [Inject]
        public IApplicationService ApplicationService { get; set; }

        #endregion

        #region Public Methods and Operators

        public IObservable<Device> Activate(string registrationId, string identifier)
        {
            var device = new Device(identifier, registrationId) { Provider = NotificationProvider };

            return Authorize(ResourceApi.Activate(device, AccessToken, ApplicationService.Version.ToString()));
        }

        public IObservable<EmptyModel> Call(string phoneNumber)
        {
            return Authorize(ResourceApi.Call(phoneNumber, AccessToken));
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

        public IObservable<EmptyModel> EndCall()
        {
            return Authorize(ResourceApi.EndCall(AccessToken));
        }

        public IObservable<List<Device>> GetAll()
        {
            var observable = ResourceApi.GetAll(AccessToken);
            return Authorize(observable);
        }

        public IObservable<EmptyModel> SendSms(string phoneNumber, string content)
        {
            return Authorize(ResourceApi.SendSms(phoneNumber, content, AccessToken));
        }

        #endregion

        #region Methods

        private static IDevicesApi CreateResourceApi()
        {
            return RestService.For<IDevicesApi>(ConfigurationManager.AppSettings[ConfigurationProperties.BaseUrl]);
        }

        #endregion
    }
}