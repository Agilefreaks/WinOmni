namespace Omnipaste.Services
{
    using System;

    public interface IUpdaterService
    {
        IObservable<int> CheckForUpdatesPeriodically();

        bool CheckIfUpdatesAvailable();

        void ApplyUpdate();
    }
}