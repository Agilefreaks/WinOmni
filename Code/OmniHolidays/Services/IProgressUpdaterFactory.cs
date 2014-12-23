namespace OmniHolidays.Services
{
    using System;
    using System.Reactive;

    public interface IProgressUpdaterFactory
    {
        IObservable<Unit> Create(double totalDurationMilliseconds, Action<double> increaseProgress);
    }
}