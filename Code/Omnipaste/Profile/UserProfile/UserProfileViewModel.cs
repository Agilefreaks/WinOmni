namespace Omnipaste.Profile.UserProfile
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Omni;
    using OmniApi.Dto;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Shell.SessionInfo;

    public class UserProfileViewModel : Screen, IUserProfileViewModel
    {
        private readonly IDevices _devicesApi;

        private readonly IOmniService _omniService;

        private IObservableCollection<DeviceDto> _devices;

        private IDisposable _subscription;

        private ContactModel _user;

        public UserProfileViewModel(
            IConfigurationService configurationService,
            IDevices devicesApi,
            IOmniService omniService)
        {
            User = new ContactModel(new UserEntity(configurationService.UserInfo));
            _devicesApi = devicesApi;
            _omniService = omniService;
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

        public string StatusText
        {
            get
            {
                ConnectionStateEnum result = _omniService.State == OmniServiceStatusEnum.Started ? ConnectionStateEnum.Connected : ConnectionStateEnum.Disconnected;
                return result.ToResourceString();
            }
        }

        public string Identifier
        {
            get
            { 
               return User.Identifier;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _subscription =
                _devicesApi.GetAll()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .Subscribe(GetDevices);
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                _subscription.Dispose();
            }

            base.OnDeactivate(close);
        }

        private void GetDevices(List<DeviceDto> deviceDtos)
        {
            Devices.AddRange(deviceDtos);
        }
    }
}