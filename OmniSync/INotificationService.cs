namespace OmniSync
{
    using System;
    using System.Threading.Tasks;
    using OmniCommon.Models;

    public interface INotificationService : IObservable<OmniMessage>
    {
        ServiceStatusEnum Status { get; }

        Task<RegistrationResult> Start();

        void Stop();
    }
}
