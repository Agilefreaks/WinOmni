namespace OmniHolidays.ExtensionMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;

    public static class ObservableExtensionMethods
    {
        public static IObservable<T> ToSequentialDelayedObservable<T>(this IEnumerable<T> output, TimeSpan delay)
        {
            var items = output as T[] ?? output.ToArray();
            return items.Select(item => Observable.Return(item).Delay(delay)).Concat();
        }
    }
}