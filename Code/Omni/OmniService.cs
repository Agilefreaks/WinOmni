namespace Omni
{
    using System.Threading.Tasks;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using OmniSync;

    public class OmniService : IOmniService
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly IDevicesApi _devicesApi;

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

        public IOmniSyncService OmniSyncService { get; set; }

        public ServiceStatusEnum Status
        {
            get
            {
                return OmniSyncService.Status;
            }
        }

        #endregion

        #region Public Methods and Operators

        public async Task<bool> Start(string communicationChannel = null)
        {
            var registrationResult = await OmniSyncService.Start();

            var deviceIdentifier = _configurationService.GetDeviceIdentifier();
            var machineName = _configurationService.GetMachineName();

            await _devicesApi.Register(deviceIdentifier, machineName);

            const string NotificationProvider = "omni_sync";
            var activationResult =
                await _devicesApi.Activate(registrationResult.Data, deviceIdentifier, NotificationProvider);

            return activationResult.Data != null;
        }

        public void Stop()
        {
            OmniSyncService.Stop();
        }

        #endregion
    }
}