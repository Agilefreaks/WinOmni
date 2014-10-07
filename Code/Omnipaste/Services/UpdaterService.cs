namespace Omnipaste.Services
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reflection;
    using System.Threading;
    using NAppUpdate.Framework;
    using NAppUpdate.Framework.Common;
    using NAppUpdate.Framework.Sources;
    using NAppUpdate.Framework.Tasks;
    using OmniCommon;
    using OmniCommon.ExtensionMethods;

    public class UpdaterService : IUpdaterService
    {
        private const string InstallerName = "OmnipasteInstaller.msi";

        private const string UpdateFeedFileName = "FeedBuilder.xml";

        private readonly TimeSpan _updateCheckInterval = TimeSpan.FromMinutes(60);
        private readonly UpdateManager _updateManager;

        protected static string RootDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);

                return Path.GetDirectoryName(path);
            }
        }

        protected static string FeedUrl
        {
            get
            {
                return string.Format(
                    "{0}{1}",
                    ConfigurationManager.AppSettings[ConfigurationProperties.UpdateSource],
                    UpdateFeedFileName);
            }
        }

        public UpdaterService()
        {
            _updateManager = UpdateManager.Instance;
            _updateManager.UpdateSource = new SimpleWebSource(FeedUrl);
            _updateManager.ReinstateIfRestarted();
        }

        public IObservable<int> CheckForUpdatesPeriodically()
        {
            var scheduler = Observable.Timer(TimeSpan.Zero, _updateCheckInterval);
            return scheduler
                .Where(_ => CheckIfUpdatesAvailable())
                .Select(_ => _updateManager.UpdatesAvailable);
        }

        public bool CheckIfUpdatesAvailable()
        {
            var completed = false;
            var autoResetEvent = new AutoResetEvent(false);
            _updateManager.BeginCheckForUpdates(
                asyncResult =>
                {
                    ((UpdateProcessAsyncResult)asyncResult).EndInvoke();
                    completed = asyncResult.IsCompleted;
                    autoResetEvent.Set();
                }, null);
            autoResetEvent.WaitOne();

            return completed && _updateManager.UpdatesAvailable > 0;
        }

        public void ApplyUpdate()
        {
            _updateManager.BeginPrepareUpdates(asyncResult =>
            {
                ((UpdateProcessAsyncResult)asyncResult).EndInvoke();
                try
                {
                    //it is necessary to do this here because the ApplyUpdates method will clear all the Tasks it has performed
                    var installerUpdateTask = _updateManager.Tasks.Cast<FileUpdateTask>()
                        .FirstOrDefault(fileUpdateTask => fileUpdateTask.LocalPath == InstallerName);

                    _updateManager.ApplyUpdates(true);
                    
                    if (installerUpdateTask == null) return;

                    var installerPath = Path.Combine(RootDirectory, installerUpdateTask.LocalPath);
                    Process.Start(installerPath);
                }
                catch (Exception exception)
                {
                    this.Log("An error occurred while trying to install software updates: " + exception);
                    throw;
                }
            }, null);
        }
    }
}