namespace Omni
{
    using System.Threading.Tasks;
    using OmniSync;

    public class OmniService : IOmniService
    {
        public IOmniSyncService OmniSyncService { get; set; }

        public OmniService(IOmniSyncService omniSyncService)
        {
            this.OmniSyncService = omniSyncService;
        }

        public async Task<bool> Start(string communicationChannel = null)
        {
            //connect to websocket using the Device Channel
            var registrationResult = await OmniSyncService.Start();

            return false;
        }

        public void Stop()
        {
            //close the websocket
            //notify webomni that the device is disconnected
            throw new System.NotImplementedException();
        }
    }
}