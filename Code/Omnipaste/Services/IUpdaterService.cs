namespace Omnipaste.Services
{
    using System;
    using Ninject;

    public interface IUpdaterService : IStartable
    {
        IObservable<UpdateInfo> UpdateObservable { get; }

        IObservable<bool> AreUpdatesAvailable(TimeSpan updateCheckInterval);

        void InstallNewVersionWhenIdle(TimeSpan idleTimeSpan);

        void InstallNewVersion();

        void CleanTemporaryFiles();

        bool NewLocalInstallerAvailable();
    }
}