namespace OmniDebug.Services
{
    using Omni;
    using OmniCommon.Models;

    public interface IOmniServiceWrapper : IOmniService
    {
        void SimulateMessage(OmniMessage omniMessage);
    }
}