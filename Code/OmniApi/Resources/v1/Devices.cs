﻿namespace OmniApi.Resources.v1
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using OmniApi.Models;
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
            IObservable<Device> Activate([Body] Device device, [Header("Authorization")] string token);

            [Post("/devices")]
            IObservable<Device> Create([Body] Device device, [Header("Authorization")] string token);

            [Put("/devices/deactivate")]
            IObservable<Device> Deactivate([Body] Device device, [Header("Authorization")] string token);

            [Get("/devices")]
            IObservable<List<Device>> GetAll([Header("Authorization")] string token);

            #endregion
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Device> Activate(string registrationId, string identifier)
        {
            var device = new Device(identifier, registrationId) { Provider = NotificationProvider };
            return Authorize(ResourceApi.Activate(device, AccessToken));
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

        #endregion
    }
}