namespace Omnipaste.ExtensionMethods
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using BugFreak;
    using NAppUpdate.Framework;

    public static class UpdateManagerExtensionMethods
    {
        public static IObservable<bool> AreUpdatesAvailable(this UpdateManager updateManager, Func<bool> updateAvailableCheck = null)
        {
            return Observable.Create<bool>(
                observer =>
                    {
                        try
                        {
                            updateManager.CleanUp();
                            updateManager.CheckForUpdates();
                            var updatesAvailable = (updateAvailableCheck ?? (() => updateManager.UpdatesAvailable > 0))();

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

        public static IObservable<bool> DownloadUpdates(this UpdateManager updateManager, Action onDownloadSuccess = null)
        {
            return Observable.Create<bool>(
                observer =>
                {
                    try
                    {
                        updateManager.PrepareUpdates();
                        if (onDownloadSuccess != null) onDownloadSuccess();
                        observer.OnNext(true);
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