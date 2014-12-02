namespace Omnipaste.Services
{
    using System;
    using System.Reactive;
    using Ninject;

    public interface IUiRefreshService : IStartable
    {
        IObservable<Unit> RefreshObservable { get; }
    }
}