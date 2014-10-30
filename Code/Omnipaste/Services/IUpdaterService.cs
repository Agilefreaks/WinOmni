namespace Omnipaste.Services
{
    using System;
    using Ninject;
    using OmniCommon.Interfaces;

    public interface IUpdaterService : IStartable, IProxyConfigurationObserver
    {
        IObservable<bool> AreUpdatesAvailable(TimeSpan updateCheckInterval);

        IObservable<bool> DownloadUpdates();

        void InstallNewVersionWhenIdle(TimeSpan idleTimeSpan);

        void InstallNewVersion();

        void CleanTemporaryFiles();

        bool NewLocalInstallerAvailable();
    }
}