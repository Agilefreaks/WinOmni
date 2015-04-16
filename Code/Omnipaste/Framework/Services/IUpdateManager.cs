namespace Omnipaste.Framework.Services
{
    using System;
    using System.Collections.Generic;
    using NAppUpdate.Framework.Sources;

    public interface IUpdateManager
    {
        IUpdateSource UpdateSource { get; set; }

        int UpdatesAvailable { get; }

        void ReinstateIfRestarted();

        void ApplyUpdates(bool restartApp);

        void PrepareUpdates();

        void CheckForUpdates();

        void CleanUp();

        IList<UpdateFileInfo> GetUpdatedFiles();

        IObservable<bool> AreUpdatesAvailable(Func<bool> newRemoteInstallerAvailable = null);

        IObservable<bool> DownloadUpdates(Action onSuccess);
    }
}
