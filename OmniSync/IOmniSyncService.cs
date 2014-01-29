namespace OmniSync
{
    using System;
    using System.Threading.Tasks;

    public interface IOmniSyncService : IObservable<OmniMessage>
    {
        RegistrationResult GetRegistrationId(string communicationChannel);

        Task<RegistrationResult> Start();

        void Stop();
    }
}
