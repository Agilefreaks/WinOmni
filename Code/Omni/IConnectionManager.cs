namespace Omni
{
    using System;
    using System.Reactive;
    using OmniSync;

    public interface IConnectionManager : IObserver<ServiceStatusEnum>
    {
        ServiceStatusEnum Status { get; }

        IObservable<ServiceStatusEnum> StatusChangedObservable { get; }

        IObservable<Unit> GoToState(ServiceStatusEnum newState);
    }
}