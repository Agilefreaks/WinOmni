namespace Omnipaste.ExtensionMethods
{
    using System;
    using System.Reactive.Linq;
    using BugFreak;

    public static class ObservableExtensionMethods
    {
        public static IObservable<T> CatchAndReport<T>(this IObservable<T> observable, T onCatch = default(T))
        {
            return observable.Catch<T, Exception>(
                exception =>
                {
                    ReportingService.Instance.BeginReport(exception);
                    return new[] { onCatch }.ToObservable();
                });
        }
    }
}