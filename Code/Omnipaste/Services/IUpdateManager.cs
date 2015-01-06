namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using NAppUpdate.Framework.Sources;
    using NAppUpdate.Framework.Tasks;

    public interface IUpdateManager
    {
        IEnumerable<IUpdateTask> Tasks { get; }

        IUpdateSource UpdateSource { get; set; }

        int UpdatesAvailable { get; }

        void ReinstateIfRestarted();

        void ApplyUpdates(bool restartApp);

        void PrepareUpdates();

        void CheckForUpdates();

        void CleanUp();

        IObservable<bool> AreUpdatesAvailable(Func<bool> newRemoteInstallerAvailable = null);

        IObservable<bool> DownloadUpdates(Action onSuccess);
    }
}
