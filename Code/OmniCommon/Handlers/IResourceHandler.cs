namespace OmniCommon.Handlers
{
    using System;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public interface IResourceHandler<out TEntity> : IHandler, IObservable<TEntity>, IObserver<OmniMessage>, IDisposable
    {
    }
}
