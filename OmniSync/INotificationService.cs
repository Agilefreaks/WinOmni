namespace OmniSync
{
    using System.Threading.Tasks;

    public interface INotificationService
    {
        ServiceStatusEnum Status { get; }

        Task<RegistrationResult> Start();

        void Stop();
    }
}
