namespace Omni
{
    using System.Threading.Tasks;
    using OmniSync;

    public interface IOmniService
    {
        Task<bool> Start(string communicationChannel = null);

        void Stop();

        ServiceStatusEnum Status { get; }
    }
}