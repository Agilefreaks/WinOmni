namespace Omnipaste.ExtensionMethods
{
    using System;
    using System.Reactive.Linq;
    using NAppUpdate.Framework;

    public static class UpdateManagerExtensionMethods
    {
        public static IObservable<bool> AreUpdatesAvailable(
            this UpdateManager updateManager,
            Func<bool> updateAvailableCheck = null)
        {
            return Observable.Start(
                () =>
                    {
                        updateManager.CleanUp();
                        updateManager.CheckForUpdates();
                        var result = (updateAvailableCheck ?? (() => updateManager.UpdatesAvailable > 0))();

                        return result;
                    });
        }

        public static IObservable<bool> DownloadUpdates(
            this UpdateManager updateManager,
            Action onDownloadSuccess = null)
        {
            return Observable.Start(
                () =>
                    {
                        updateManager.PrepareUpdates();
                        if (onDownloadSuccess != null) onDownloadSuccess();

                        return true;
                    });

        }
    }
}