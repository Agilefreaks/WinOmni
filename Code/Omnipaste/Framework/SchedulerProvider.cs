namespace Omnipaste.Framework
{
    using System.Reactive.Concurrency;

    public static class SchedulerProvider
    {
        private static IScheduler _dispatcher;

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
    }
}