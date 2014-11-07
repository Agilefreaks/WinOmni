namespace OmniCommon.Helpers
{
    using System.Reactive.Concurrency;

    public class SchedulerProvider
    {
        private static IScheduler _defaultScheduler, _dispatcherScheduler;

        public static IScheduler Default
        {
            get
            {
                return _defaultScheduler ?? Scheduler.Default;
            }

            set
            {
                _defaultScheduler = value;
            }
        }

        public static IScheduler Dispatcher
        {
            get
            {
                //ToDO: might want to replace this with TaskScheduler.FromCurrentSynchronizationContext() which is the WPF dispatcher when you're running a WPF application
                return _dispatcherScheduler ?? DispatcherScheduler.Current;
            }
            set
            {
                _dispatcherScheduler = value;
            }
        }
    }
}