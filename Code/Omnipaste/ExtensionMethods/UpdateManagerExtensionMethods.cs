namespace Omnipaste.ExtensionMethods
{
    using System;
    using System.Reactive.Linq;
    using NAppUpdate.Framework;
    using OmniCommon.Helpers;

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
                    }, SchedulerProvider.Default);
        }

        public static IObservable<bool> DownloadUpdates(
            this UpdateManager updateManager)
        {
            return Observable.Start(
                () =>
                    {
                        updateManager.PrepareUpdates();
                        return true;
                    }, SchedulerProvider.Default);

        }
    }
}