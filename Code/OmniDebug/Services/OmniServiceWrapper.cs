namespace OmniDebug.Services
{
    using Omni;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniSync;

    public class OmniServiceWrapper : OmniService, IOmniServiceWrapper
    {
        public OmniServiceWrapper(IConfigurationService configurationService, IWebsocketConnectionFactory websocketConnectionFactory)
            : base(configurationService, websocketConnectionFactory)
        {
        }

        public void SimulateMessage(OmniMessage omniMessage)
        {
            ((WebsocketConnectionWrapper)WebsocketConnection).SimulateMessage(omniMessage);
        }
    }
}