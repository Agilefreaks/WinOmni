namespace OmniDebug.Services
{
    using Omni;
    using OmniCommon.Models;

    public interface IConnectionManagerWrapper : IConnectionManager
    {
        void SimulateMessage(OmniMessage omniMessage);
    }
}