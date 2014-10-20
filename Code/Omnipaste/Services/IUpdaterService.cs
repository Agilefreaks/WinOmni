namespace Omnipaste.Services
{
    using System;

    public interface IUpdaterService
    {
        IObservable<bool> CreateUpdateReadyObservable(TimeSpan updateCheckInterval);

        bool CheckIfUpdatesAvailable();

        void ApplyUpdate();

        void ApplyUpdateWhenIdle(TimeSpan idleTimeSpan);
    }
}