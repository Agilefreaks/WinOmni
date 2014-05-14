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
            await _devicesAPI.Register(_configurationService.GetDeviceIdentifier(), System.Environment.MachineName);
            var activationResult = await _devicesAPI.Activate(registrationResult.Data, _configurationService.GetDeviceIdentifier(), "omni_sync");

            return activationResult.Data != null;
        }

        public void Stop()
        {
            OmniSyncService.Stop();
        }
    }
}