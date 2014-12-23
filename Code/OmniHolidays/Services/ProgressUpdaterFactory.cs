namespace OmniHolidays.Services
{
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using OmniHolidays.ExtensionMethods;

    public class ProgressUpdaterFactory : IProgressUpdaterFactory
    {
        public const int UpdateIntervalMilliseconds = 42;

        public IObservable<Unit> Create(
            double totalDurationMilliseconds,
            Action<double> increaseProgress)
        {
            var totalProgressUpdates = (int)totalDurationMilliseconds / UpdateIntervalMilliseconds + 1;
            var progressIncrement = (double)100 / totalProgressUpdates;
            var updateInterval = TimeSpan.FromMilliseconds(UpdateIntervalMilliseconds);
            return
                Enumerable.Range(0, totalProgressUpdates)
                    .ToSequentialDelayedObservable(updateInterval)
                    .Do(_ => increaseProgress(progressIncrement))
                    .Buffer(totalProgressUpdates)
                    .Select(_ => Unit.Default);
        }
    }
}