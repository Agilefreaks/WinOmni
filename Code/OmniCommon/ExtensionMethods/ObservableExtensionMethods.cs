﻿namespace OmniCommon.ExtensionMethods
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using OmniCommon.Helpers;

    public static class ObservableExtensionMethods
    {
        #region Static Fields

        public static readonly Func<int, TimeSpan> ExponentialBackoff = n => TimeSpan.FromSeconds(Math.Pow(n, 2));

        #endregion

        #region Public Properties

        public static IExceptionReporter ExceptionReporter { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void RunToCompletion<T>(this IObservable<T> observable, Action<T> onCompletion = null, Action<Exception> onError = null)
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            Task.Factory.StartNew(() => observable.RunToCompletionSynchronous(onCompletion, onError, dispatcher));
        }

        public static IObservable<T> RetryAfter<T>(
            this IObservable<T> source,
            TimeSpan interval,
            IScheduler scheduler = null)
        {
            Func<int, TimeSpan> constantBackoff = _ => interval;
            return source.RetryWithBackoffStrategy(null, constantBackoff, null, scheduler);
        }

        public static IObservable<T> RetryWithBackoffStrategy<T>(
            this IObservable<T> source,
            int? retryCount = null,
            Func<int, TimeSpan> strategy = null,
            Func<Exception, bool> retryOnError = null,
            IScheduler scheduler = null)
        {
            strategy = strategy ?? ExponentialBackoff;
            scheduler = scheduler ?? SchedulerProvider.Default;

            if (retryOnError == null) retryOnError = e => true;

            var attempt = 0;

            var wrappedObservable = Observable.Defer(
                () => ((++attempt == 1) ? source : source.DelaySubscription(strategy(attempt - 1), scheduler))
                          .Select(item => new Tuple<bool, T, Exception>(true, item, null))
                          .Catch<Tuple<bool, T, Exception>, Exception>(
                              e =>
                              retryOnError(e)
                                  ? Observable.Throw<Tuple<bool, T, Exception>>(e)
                                  : Observable.Return(new Tuple<bool, T, Exception>(false, default(T), e))));
            var retryObservable = retryCount.HasValue ? wrappedObservable.Retry(retryCount.Value) : wrappedObservable.Retry();

            return retryObservable.SelectMany(t => t.Item1 ? Observable.Return(t.Item2) : Observable.Throw<T>(t.Item3));
        }


        public static IDisposable SubscribeAndHandleErrors<T>(this IObservable<T> observable)
        {
            return observable.Subscribe(_ => { }, OnExceptionEncountered);
        }

        public static IDisposable SubscribeAndHandleErrors<T>(this IObservable<T> observable, Action<T> onNext)
        {
            return observable.Subscribe(onNext, OnExceptionEncountered);
        }

        #endregion

        #region Methods

        private static void OnExceptionEncountered(Exception exception)
        {
            var reporter = ExceptionReporter ?? NullExceptionReporter.Instance;
            SimpleLogger.Log("Exception encountered: " + exception);
            reporter.Report(exception);
        }

        private static void RunToCompletionSynchronous<T>(
            this IObservable<T> observable,
            Action<T> onCompletion,
            Action<Exception> onError,
            Dispatcher dispatcher)
        {
            var task = observable.ToTask();
            task.Wait();
            if (task.IsFaulted)
            {
                OnExceptionEncountered(task.Exception);
                dispatcher.RunToCompletionSynchronous(onError, task.Exception);
            }
            else
            {
                dispatcher.RunToCompletionSynchronous(onCompletion, task.Result);
            }
        }

        private static void RunToCompletionSynchronous<TData>(this Dispatcher dispatcher, Action<TData> action, TData data)
        {
            if (action != null)
            {
                dispatcher.Invoke(() => action(data));
            }
        }

        #endregion
    }
}