namespace OmniDebug
{
    using OmniCommon.Models;
    using OmniSync;

    public interface IWebsocketConnectionWrapper : IWebsocketConnection
    {
        void SimulateMessage(OmniMessage omniMessage);
    }
}