namespace Omnipaste.Services
{
    using System;
    using Ninject;

    public interface IUpdaterService : IStartable
    {
        IObservable<bool> AreUpdatesAvailable(TimeSpan updateCheckInterval);

        IObservable<bool> DownloadUpdates();

        void InstallNewVersionWhenIdle(TimeSpan idleTimeSpan);

        void InstallNewVersion();

        void CleanTemporaryFiles();

        bool NewLocalInstallerAvailable();
    }
}