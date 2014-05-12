namespace OmniSync
{
    using System.Threading.Tasks;

    public interface IOmniSyncService
    {
        ServiceStatusEnum Status { get; }

        Task<RegistrationResult> Start();

        void Stop();
    }
}
