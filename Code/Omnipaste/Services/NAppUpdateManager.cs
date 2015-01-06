namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using NAppUpdate.Framework;
    using NAppUpdate.Framework.Sources;
    using NAppUpdate.Framework.Tasks;
    using OmniCommon.Helpers;

    public class NAppUpdateManager : IUpdateManager
    {
        private readonly UpdateManager _updateManager;

        public NAppUpdateManager()
        {
            _updateManager = UpdateManager.Instance;
        }

        public IEnumerable<IUpdateTask> Tasks
        {
            get
            {
                return _updateManager.Tasks;
            }
        }

        public IUpdateSource UpdateSource
        {
            get
            {
                return _updateManager.UpdateSource;
            }
            set
            {
                _updateManager.UpdateSource = value;
            }
        }

        public int UpdatesAvailable
        {
            get
            {
                return _updateManager.UpdatesAvailable;
            }
        }

        public void ReinstateIfRestarted()
        {
            _updateManager.ReinstateIfRestarted();
        }

        public IObservable<bool> AreUpdatesAvailable(Func<bool> updateAvailableCheck = null)
        {
            return Observable.Start(
                () =>
                    {
                        _updateManager.CleanUp();
                        _updateManager.CheckForUpdates();
                        var result = (updateAvailableCheck ?? (() => _updateManager.UpdatesAvailable > 0))();

                        return result;
                    }, SchedulerProvider.Default);
        }

        public void ApplyUpdates(bool relaunchApplication)
        {
            _updateManager.ApplyUpdates(relaunchApplication);
        }

        public void PrepareUpdates()
        {
            _updateManager.PrepareUpdates();
        }

        public IList<UpdateFileInfo> GetUpdatedFiles()
        {
            return
                _updateManager.Tasks.Where(task => task is FileUpdateTask)
                    .Cast<FileUpdateTask>()
                    .Select(t => new UpdateFileInfo { LocalPath = t.LocalPath, Version = t.Version })
                    .ToList();
        }

        public void CheckForUpdates()
        {
            _updateManager.CheckForUpdates();
        }

        public void CleanUp()
        {
            _updateManager.CleanUp();
        }

        public IObservable<bool> DownloadUpdates(Action onSuccess)
        {
            return Observable.Start(
                () =>
                    {
                        _updateManager.PrepareUpdates();
                        onSuccess();

                        return true;
                    }, SchedulerProvider.Default);

        }
    }
}