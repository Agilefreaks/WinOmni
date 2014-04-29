namespace Omni
{
    using System.Threading.Tasks;
    using OmniApi.Resources;
    using OmniCommon.DataProviders;
    using OmniSync;

    public class OmniService : IOmniService
    {
        private readonly IDevicesAPI _devicesAPI;

        private readonly IConfigurationProvider _configurationProvider;

        private readonly string _deviceIdentifier;

        public INotificationService NotificationService { get; set; }
        
        public OmniService(INotificationService notificationService, IDevicesAPI devicesAPI, IConfigurationProvider configurationProvider)
        {
            NotificationService = notificationService;
            _devicesAPI = devicesAPI;
            _configurationProvider = configurationProvider;

            _deviceIdentifier = _configurationProvider["deviceIdentifier"];
        }

        public async Task<bool> Start(string communicationChannel = null)
        {
            var registrationResult = await NotificationService.Start();
            var activationResult = await _devicesAPI.Activate(registrationResult.Data, _deviceIdentifier, "omni_sync");

            return false;
        }

        public void Stop()
        {
            NotificationService.Stop();
        }
    }
}