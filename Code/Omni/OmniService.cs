using OmniCommon.Interfaces;

namespace Omni
{
    using System.Threading.Tasks;
    using OmniApi.Resources;
    using OmniSync;

    public class OmniService : IOmniService
    {
        private readonly IDevicesAPI _devicesApi;

        private readonly IConfigurationService _configurationService;

        public IOmniSyncService OmniSyncService { get; set; }
        
        public OmniService(IOmniSyncService omniSyncService, IDevicesAPI devicesApi, IConfigurationService configurationService)
        {
            OmniSyncService = omniSyncService;
            _devicesApi = devicesApi;
            _configurationService = configurationService;
        }

        public async Task<bool> Start(string communicationChannel = null)
        {
            var registrationResult = await OmniSyncService.Start();
            
            var deviceIdentifier = _configurationService.GetDeviceIdentifier();
            var machineName = _configurationService.GetMachineName();
            
            await _devicesApi.Register(deviceIdentifier, machineName);

            const string NotificationProvider = "omni_sync";
            var activationResult = await _devicesApi.Activate(registrationResult.Data, deviceIdentifier, NotificationProvider);

            return activationResult.Data != null;
        }

        public void Stop()
        {
            OmniSyncService.Stop();
        }
    }
}