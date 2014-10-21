namespace Omnipaste.ExtensionMethods
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading;
    using BugFreak;
    using NAppUpdate.Framework;

    public static class UpdateManagerExtensionMethods
    {
        public static IObservable<bool> DownloadUpdates(this UpdateManager updateManager)
        {
            return Observable.Create<bool>(
                observer =>
                {
                    try
                    {
                        var autoResetEvent = new AutoResetEvent(false);
                        var couldDownloadFiles = false;
                        updateManager.BeginPrepareUpdates(
                            asyncResult =>
                            {
                                couldDownloadFiles = asyncResult.CompleteSafely();
                                autoResetEvent.Set();
                            },
                            null);
                        autoResetEvent.WaitOne();

                        observer.OnNext(couldDownloadFiles);
                    }
                    catch (Exception exception)
                    {
                        ReportingService.Instance.BeginReport(exception);
                        observer.OnNext(false);
                    }

                    observer.OnCompleted();

                    return Disposable.Empty;
                });

        }

        public static IObservable<bool> AreUpdatesAvailable(this UpdateManager updateManager, Func<bool> updateAvailableCheck = null)
        {
            updateAvailableCheck = updateAvailableCheck ?? (() => updateManager.UpdatesAvailable > 0);
            return Observable.Create<bool>(
                observer =>
                {
                    try
                    {
                        var autoResetEvent = new AutoResetEvent(false);
                        var updatesAvailable = false;
                        updateManager.BeginCheckForUpdates(
                            asyncResult =>
                                {
                                    updatesAvailable = asyncResult.CompleteSafely() && updateAvailableCheck();
                                    autoResetEvent.Set();
                                },
                            null);
                        autoResetEvent.WaitOne();

                        observer.OnNext(updatesAvailable);
                    }
                    catch (Exception exception)
                    {
                        ReportingService.Instance.BeginReport(exception);
                        observer.OnNext(false);
                    }

                    observer.OnCompleted();

                    return Disposable.Empty;
                });
        }
    }
}