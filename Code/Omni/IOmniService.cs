namespace Omni
{
    using System;
    using System.Threading.Tasks;
    using OmniSync;

    public interface IOmniService : IObservable<ServiceStatusEnum>
    {
        Task<bool> Start(string communicationChannel = null);

        void Stop();

        ServiceStatusEnum Status { get; }
    }
}