using OmniCommon.Interfaces;

namespace Omni
{
    using System.Threading.Tasks;
    using OmniApi.Resources;
    using OmniSync;

    public class OmniService : IOmniService
    {
        private readonly IDevicesAPI _devicesAPI;

        private readonly IConfigurationService _configurationService;

        private readonly string _deviceIdentifier;

        public IOmniSyncService OmniSyncService { get; set; }
        
        public OmniService(IOmniSyncService omniSyncService, IDevicesAPI devicesAPI, IConfigurationService configurationService)
        {
            OmniSyncService = omniSyncService;
            _devicesAPI = devicesAPI;
            _configurationService = configurationService;
        }

        public async Task<bool> Start(string communicationChannel = null)
        {
            var registrationResult = await OmniSyncService.Start();
            var deviceIdentifier = _configurationService.GetDeviceIdentifier();
            var machineName = _configurationService.GetMachineName();
            const string notificationProvider = "omni_sync";

            await _devicesAPI.Register(deviceIdentifier, machineName);
            var activationResult = await _devicesAPI.Activate(registrationResult.Data, deviceIdentifier, notificationProvider);

            return activationResult.Data != null;
        }

        public void Stop()
        {
            OmniSyncService.Stop();
        }
    }
}