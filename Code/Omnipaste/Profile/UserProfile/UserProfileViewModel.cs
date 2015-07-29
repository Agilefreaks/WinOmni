namespace Omnipaste.Profile.UserProfile
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using OmniApi.Dto;
    using OmniApi.Resources.v1;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;

    public class UserProfileViewModel : Screen, IUserProfileViewModel
    {
        private readonly IDevices _devicesApi;

        private IObservableCollection<DeviceDto> _devices;

        private IDisposable _subscription;

        private ContactModel _user;

        public UserProfileViewModel(IConfigurationService configurationService, IDevices devicesApi)
        {
            User = new ContactModel(new UserEntity(configurationService.UserInfo));
            _devicesApi = devicesApi;
            Devices = new BindableCollection<DeviceDto>();
        }

        public IObservableCollection<DeviceDto> Devices
        {
            get
            {
                return _devices;
            }
            set
            {
                _devices = value;
                NotifyOfPropertyChange();
            }
        }

        public ContactModel User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                NotifyOfPropertyChange();
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _subscription =
                _devicesApi.GetAll()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(GetDevices);
        }

        private void GetDevices(List<DeviceDto> deviceDtos)
        {
            Devices.AddRange(deviceDtos);
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                // Todo: clear subscrpiton
                _subscription.Dispose();
            }

            base.OnDeactivate(close);
        }
    }
}