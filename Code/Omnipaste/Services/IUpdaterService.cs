namespace Omnipaste.Services
{
    using System;
    using Ninject;
    using Omnipaste.Framework.Entities;

    public interface IUpdaterService : IStartable
    {
        IObservable<UpdateEntity> UpdateObservable { get; }

        IObservable<bool> AreUpdatesAvailable(TimeSpan updateCheckInterval);

        void InstallNewVersionWhenIdle(TimeSpan idleTimeSpan);

        void InstallNewVersion(bool startMinimized = false);

        void CleanTemporaryFiles();

        bool NewLocalInstallerAvailable();
    }
}