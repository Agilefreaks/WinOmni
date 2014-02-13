namespace OmniSync
{
    using System;
    using System.Threading.Tasks;
    using OmniCommon.Models;

    public interface INotificationService : IObservable<OmniMessage>
    {
        Task<RegistrationResult> Start();

        void Stop();
    }
}
