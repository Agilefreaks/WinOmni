namespace Omnipaste.Framework
{
    using System.Reactive.Concurrency;

    public static class SchedulerProvider
    {
        private static IScheduler _dispatcher;

        private static IScheduler _defaultScheduler;

        public static IScheduler Dispatcher
        {
            get
            {
                return _dispatcher ?? (_dispatcher = DispatcherScheduler.Current);
            }
            set
            {
                _dispatcher = value;
            }
        }

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
    }
}