﻿namespace Omnipaste.Services
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reactive.Linq;
    using System.Reflection;
    using System.Threading;
    using BugFreak;
    using NAppUpdate.Framework;
    using NAppUpdate.Framework.Common;
    using NAppUpdate.Framework.Sources;
    using NAppUpdate.Framework.Tasks;
    using OmniCommon;
    using Omnipaste.Framework;

    public class UpdaterService : IUpdaterService
    {
        private const string InstallerName = "OmnipasteInstaller.msi";

        private const string UpdateFeedFileName = "FeedBuilder.xml";

        private const string MSIExec = "msiexec.exe";

        private readonly TimeSpan _initialUpdateCheckDelay = TimeSpan.FromSeconds(15);
        private readonly UpdateManager _updateManager;

        private IDisposable _systemIdleObserver;

        public ISystemIdleService SystemIdleService { get; set; }

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

        protected static string AppName
        {
            get
            {
                return ConfigurationManager.AppSettings[ConfigurationProperties.AppName];
            }
        }

        protected static string InstallerTemporaryFolder
        {
            get
            {
                return Path.Combine(Path.GetTempPath(), AppName);
            }
        }

        protected static string MsiTemporaryPath
        {
            get
            {
                return Path.Combine(InstallerTemporaryFolder, InstallerName);
            }
        }

        public UpdaterService(ISystemIdleService systemIdleService)
        {
            SystemIdleService = systemIdleService;
            _updateManager = UpdateManager.Instance;
            _updateManager.UpdateSource = new SimpleWebSource(FeedUrl) { Proxy = WebRequest.GetSystemWebProxy() };
            _updateManager.ReinstateIfRestarted();
        }

        public IObservable<bool> CreateUpdateReadyObservable(TimeSpan updateCheckInterval)
        {
            return Observable.Timer(_initialUpdateCheckDelay, updateCheckInterval)
                .Select(_ => CheckIfUpdatesAvailable());
        }

        public bool CheckIfUpdatesAvailable()
        {
            var completed = false;
            var autoResetEvent = new AutoResetEvent(false);
            CleanTemporaryFiles();
            _updateManager.BeginCheckForUpdates(
                asyncResult =>
                {
                    HandleAsyncResultSafely(asyncResult);
                    completed = asyncResult.IsCompleted;
                    autoResetEvent.Set();
                }, null);
            autoResetEvent.WaitOne();

            return completed && _updateManager.UpdatesAvailable > 0;
        }

        public void ApplyUpdateWhenIdle(TimeSpan systemIdleThreshold)
        {
            DisposeSystemIdleObserver();
            _systemIdleObserver =
                SystemIdleService.CreateSystemIdleObservable(systemIdleThreshold)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .Where(systemIsIdle => systemIsIdle)
                    .Subscribe(
                        _ =>
                            {
                                DisposeSystemIdleObserver();
                                ApplyUpdate();
                            });
        }

        public void ApplyUpdate()
        {
            _updateManager.BeginPrepareUpdates(
                asyncResult =>
                {
                    HandleAsyncResultSafely(asyncResult);
                    OnUpdatesPrepared();
                }, null);
        }

        private static bool NewMsiVersionAvailable(FileUpdateTask installerUpdateTask)
        {
            if (installerUpdateTask == null) return false;

            Version msiVersion;
            Version.TryParse(installerUpdateTask.Version, out msiVersion);
            var exeVersion = Assembly.GetEntryAssembly().GetName().Version;

            return msiVersion > exeVersion;
        }

        private static void CleanTemporaryFiles()
        {
            if (Directory.Exists(InstallerTemporaryFolder))
            {
                Directory.Delete(InstallerTemporaryFolder, true);
            }
        }

        private static void HandleAsyncResultSafely(IAsyncResult asyncResult)
        {
            if (!asyncResult.IsCompleted) return;

            try
            {
                ((UpdateProcessAsyncResult)asyncResult).EndInvoke();
            }
            catch (Exception exception)
            {
                ReportingService.Instance.BeginReport(exception);
            }
        }

        private void OnUpdatesPrepared()
        {
            //it is necessary to do this here because the ApplyUpdates method will clear all the Tasks it has performed
            var installerUpdateTask =
                _updateManager.Tasks.Where(task => task is FileUpdateTask)
                    .Cast<FileUpdateTask>()
                    .FirstOrDefault(fileUpdateTask => fileUpdateTask.LocalPath == InstallerName);

            if (NewMsiVersionAvailable(installerUpdateTask))
            {
                InstallNewVersion(installerUpdateTask);
            }
        }

        private void InstallNewVersion(FileUpdateTask installerUpdateTask)
        {
            try
            {
                _updateManager.ApplyUpdates(true);
                if (!Directory.Exists(InstallerTemporaryFolder))
                {
                    Directory.CreateDirectory(InstallerTemporaryFolder);
                    //Move new msi to a temp file as the app directory might get uninstalled
                    var installerPath = Path.Combine(RootDirectory, installerUpdateTask.LocalPath);
                    File.Copy(installerPath, MsiTemporaryPath);
                }

                Process.Start(MSIExec, string.Format("/i {0} /qn", MsiTemporaryPath));
            }
            catch (Exception exception)
            {
                ReportingService.Instance.BeginReport(exception);
            }
        }

        private void DisposeSystemIdleObserver()
        {
            if (_systemIdleObserver != null)
            {
                _systemIdleObserver.Dispose();
            }
        }
    }
}