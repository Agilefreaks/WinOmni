namespace Omnipaste.Services
{
    using System;

    public interface IUpdaterService
    {
        void SetupAutoUpdate(TimeSpan? updateCheckInterval = null, TimeSpan? systemIdleThreshold = null);

        IObservable<bool> AreUpdatesAvailable(TimeSpan updateCheckInterval);

        IObservable<bool> DownloadUpdates();

        void InstallNewVersionWhenIdle(TimeSpan idleTimeSpan);

        void InstallNewVersion();

        void CleanTemporaryFiles();

        bool NewLocalInstallerAvailable();
    }
}