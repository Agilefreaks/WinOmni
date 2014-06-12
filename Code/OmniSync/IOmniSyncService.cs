namespace OmniSync
{
    using System;
    using System.Threading.Tasks;

    public interface IOmniSyncService : IObservable<ServiceStatusEnum>
    {
        ServiceStatusEnum Status { get; }

        Task<RegistrationResult> Start();

        void Stop();

        event EventHandler<ServiceStatusEventArgs> ConnectivityChanged;
    }
}
