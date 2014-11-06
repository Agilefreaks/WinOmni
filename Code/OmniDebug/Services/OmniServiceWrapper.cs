namespace OmniDebug.Services
{
    using Omni;
    using OmniCommon.Models;

    public class OmniServiceWrapper : OmniService, IOmniServiceWrapper
    {
        public void SimulateMessage(OmniMessage omniMessage)
        {
            ((WebsocketConnectionWrapper)WebsocketConnection).SimulateMessage(omniMessage);
        }
    }
}