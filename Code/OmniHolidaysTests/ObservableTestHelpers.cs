namespace OmniHolidaysTests
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using OmniCommon.Helpers;

    public static class ObservableTestHelpers
    {
        public static IObservable<T> WaitForProperty<T>(this T subject, string propertyName)
            where T : INotifyPropertyChanged
        {
            return
                Observable.FromEventPattern(subject, "PropertyChanged", SchedulerProvider.Default)
                    .Where(e => ((PropertyChangedEventArgs)e.EventArgs).PropertyName == propertyName)
                    .Select(_ => subject);
        }
    }
}