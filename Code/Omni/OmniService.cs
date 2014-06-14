namespace Omni
{
    using System;
    using System.Threading.Tasks;
    using OmniApi.Models;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using OmniSync;
    using RestSharp;

    public class OmniService : IOmniService
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly IDevicesApi _devicesApi;

        private IOmniSyncService _omniSyncService;

        private IDisposable _omniSyncStatusObserver;

        #endregion

        #region Constructors and Destructors

        public OmniService(
            IOmniSyncService omniSyncService,
            IDevicesApi devicesApi,
            IConfigurationService configurationService)
        {
            OmniSyncService = omniSyncService;
            _devicesApi = devicesApi;
            _configurationService = configurationService;
        }

        #endregion

        #region Public Properties

        public IOmniSyncService OmniSyncService
        {
            get
            {
                return _omniSyncService;
            }
            set
            {
                if (_omniSyncStatusObserver != null)
                {
                    _omniSyncStatusObserver.Dispose();
                }

                _omniSyncService = value;

                _omniSyncStatusObserver = _omniSyncService.Subscribe(x => Status = x);
            }
        }

        public ServiceStatusEnum Status { get; set; }

        #endregion

        #region Public Methods and Operators

        public async Task<bool> Start(string communicationChannel = null)
        {
            var omniSyncRegistrationResult = await OmniSyncService.Start();

            var deviceIdentifier = await RegisterDevice();

            var activationResult = await ActivateDevice(omniSyncRegistrationResult, deviceIdentifier);

            return activationResult.Data != null;
        }

        public void Stop()
        {
            OmniSyncService.Stop();
        }

        #endregion

        #region Methods

        private async Task<IRestResponse<Device>> ActivateDevice(
            RegistrationResult omniSyncRegistrationResult,
            string deviceIdentifier)
        {
            const string NotificationProvider = "omni_sync";
            var activationResult =
                await _devicesApi.Activate(omniSyncRegistrationResult.Data, deviceIdentifier, NotificationProvider);
            return activationResult;
        }

        private async Task<string> RegisterDevice()
        {
            var deviceIdentifier = _configurationService.GetDeviceIdentifier();
            var machineName = _configurationService.GetMachineName();

            await _devicesApi.Register(deviceIdentifier, machineName);
            return deviceIdentifier;
        }

        #endregion
    }
}