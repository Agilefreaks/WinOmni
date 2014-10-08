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

        private const string MSIExec = "msiexec.exe";

        private readonly TimeSpan _updateCheckInterval = TimeSpan.FromMinutes(60);
        private readonly TimeSpan _initialUpdateCheckDelay = TimeSpan.FromSeconds(15);
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
            var scheduler = Observable.Timer(_initialUpdateCheckDelay, _updateCheckInterval);
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
                    var installerUpdateTask =
                        _updateManager.Tasks.Where(task => task is FileUpdateTask)
                            .Cast<FileUpdateTask>()
                            .FirstOrDefault(fileUpdateTask => fileUpdateTask.LocalPath == InstallerName);

                    if (CheckIfNewMsiVersionAvailable(installerUpdateTask))
                    {
                        InstallNewVersion(installerUpdateTask);
                    }
                }
                catch (Exception exception)
                {
                    this.Log("An error occurred while trying to install software updates: " + exception);
                    throw;
                }
            }, null);
        }

        private static bool CheckIfNewMsiVersionAvailable(FileUpdateTask installerUpdateTask)
        {
            if (installerUpdateTask == null) return false;

            Version msiVersion;
            Version.TryParse(installerUpdateTask.Version, out msiVersion);
            var exeVersion = Assembly.GetEntryAssembly().GetName().Version;

            return msiVersion > exeVersion;
        }

        private void InstallNewVersion(FileUpdateTask installerUpdateTask)
        {
            //Download new msi to app root folder
            _updateManager.ApplyUpdates(true);
            var installerPath = Path.Combine(RootDirectory, installerUpdateTask.LocalPath);
            Process.Start(MSIExec, string.Format("/package {0} /quiet", installerPath));
        }
    }
}