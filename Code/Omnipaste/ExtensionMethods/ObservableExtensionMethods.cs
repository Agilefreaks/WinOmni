namespace Omnipaste.ExtensionMethods
{
    using System;
    using BugFreak;

    public static class ObservableExtensionMethods
    {
        public static IDisposable SubscribeAndHandleErrors<T>(this IObservable<T> observable)
        {
            return observable.Subscribe(_ => { }, exception => ReportingService.Instance.BeginReport(exception));
        }

        public static IDisposable SubscribeAndHandleErrors<T>(this IObservable<T> observable, Action<T> onNext)
        {
            return observable.Subscribe(onNext, exception => ReportingService.Instance.BeginReport(exception));
        }
    }
}