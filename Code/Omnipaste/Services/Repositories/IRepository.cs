namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;

    public interface IRepository<T>
    {
        IObservable<RepositoryOperation<T>> OperationObservable { get; }

        IObservable<RepositoryOperation<T>> Save(T item);

        IObservable<RepositoryOperation<T>> Delete(object id);

        IObservable<IEnumerable<T>> GetAll();

        IObservable<IEnumerable<T>> GetAll(Func<T, bool> include);
    }
}
