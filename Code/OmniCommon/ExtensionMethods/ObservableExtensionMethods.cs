namespace OmniCommon.ExtensionMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading.Tasks;
    using OmniCommon.Helpers;
    using OmniCommon.Settings;

    public static class ObservableExtensionMethods
    {
        #region Static Fields

        public static readonly Func<int, TimeSpan> ExponentialBackoff = n => TimeSpan.FromSeconds(Math.Pow(n, 2));

        #endregion

        #region Public Methods and Operators

        public static IObservable<T> RetryAfter<T>(
            this IObservable<T> source,
            TimeSpan interval,
            int? retryCount = null,
            IScheduler scheduler = null)
        {
            Func<int, TimeSpan> constantBackoff = _ => interval;
            return source.RetryWithBackoffStrategy(retryCount, constantBackoff, null, scheduler);
        }

        public static IObservable<T> RetryUntil<T>(
            this IObservable<T> source,
            Func<T, bool> predicate,
            TimeSpan? interval = null,
            int? retryCount = null,
            Func<Exception, bool> retryOnError = null,
            IScheduler scheduler = null)
        {
            var retryStrategy = interval.HasValue ? (_ => interval.Value) : ((Func<int, TimeSpan>)null);
            scheduler = scheduler ?? SchedulerProvider.Default;
            Func<Exception, bool> wrappedRetryOnError =
                exception => (exception is RetryException) || (retryOnError == null || retryOnError(exception));

            return
                source.Select(
                    newValue =>
                    predicate(newValue)
                        ? Observable.Return(newValue, scheduler)
                        : Observable.Throw<T>(new RetryException()))
                    .Switch()
                    .RetryWithBackoffStrategy(retryCount, retryStrategy, wrappedRetryOnError, scheduler);
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

        public static void RunToCompletion<T>(
            this IObservable<T> observable,
            Action<T> onCompletion = null,
            Action<Exception> onError = null,
            IDispatcher dispatcher = null)
        {
            Task.Factory.StartNew(() => observable.RunToCompletionSynchronous(onCompletion, onError, dispatcher));
        }

        public static void RunToCompletionSynchronous<T>(
            this IObservable<T> observable,
            Action<T> onCompletion = null,
            Action<Exception> onError = null,
            IDispatcher dispatcher = null)
        {
            onCompletion = onCompletion ?? (_ => { });
            onError = onError ?? (_ => { });
            dispatcher = dispatcher ?? new ImmediateDispatcher();
            var task = observable.ToTask();
            Exception caughtException;
            try
            {
                task.Wait();
                caughtException = task.Exception;
            }
            catch (Exception exception)
            {
                caughtException = exception;
            }
            if (task.IsFaulted || caughtException != null)
            {
                OnExceptionEncountered(task.Exception);
                dispatcher.Dispatch(onError, task.Exception);
            }
            else
            {
                dispatcher.Dispatch(onCompletion, task.Result);
            }
        }

        public static IDisposable SubscribeToSettingChange<T>(
            this IObservable<SettingsChangedData> settingsChangedObservable,
            string propertyName,
            Action<T> onChangeAction,
            IScheduler subscribeScheduler = null,
            IScheduler observeScheduler = null)
        {
            subscribeScheduler = subscribeScheduler ?? SchedulerProvider.Default;
            observeScheduler = observeScheduler ?? SchedulerProvider.Default;

            return
                settingsChangedObservable.Where(changeData => changeData.SettingName == propertyName)
                    .Select(changeData => (T)changeData.NewValue)
                    .SubscribeOn(subscribeScheduler)
                    .ObserveOn(observeScheduler)
                    .SubscribeAndHandleErrors(onChangeAction);
        }

        public static IObservable<T> ReportErrors<T>(this IObservable<T> source, IScheduler scheduler = null)
        {
            scheduler = scheduler ?? SchedulerProvider.Default;
            return source.Catch<T, Exception>(
                exception =>
                {
                    ExceptionReporter.Instance.Report(exception);
                    return Observable.Throw<T>(exception, scheduler);
                });
        }

        public static IDisposable SubscribeAndHandleErrors<T>(this IObservable<T> observable)
        {
            return observable.Subscribe(_ => { }, OnExceptionEncountered);
        }

        public static IDisposable SubscribeAndHandleErrors<T>(this IObservable<T> observable, Action<T> onNext)
        {
            return observable.Subscribe(onNext, OnExceptionEncountered);
        }

        public static IObservable<T> ToSequentialDelayedObservable<T>(this IEnumerable<T> output, TimeSpan delay)
        {
            var items = output as T[] ?? output.ToArray();
            return items.Select(item => Observable.Return(item).Delay(delay)).Concat();
        }

        #endregion

        #region Methods

        private static void OnExceptionEncountered(Exception exception)
        {
            SimpleLogger.Log("Exception encountered: " + exception);
            ExceptionReporter.Instance.Report(exception);
        }

        #endregion

        #region Classes

        private sealed class RetryException : Exception
        {
            public RetryException()
                : base("Predicate did not match")
            {
            }
        }

        #endregion
    }
}