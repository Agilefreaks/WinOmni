namespace OmniDebug.Services
{
    using Omni;
    using OmniCommon.Models;

    public class ConnectionManagerWrapper : ConnectionManager, IConnectionManagerWrapper
    {
        public void SimulateMessage(OmniMessage omniMessage)
        {
            ((WebsocketConnectionWrapper)WebsocketConnection).SimulateMessage(omniMessage);
        }
    }
}