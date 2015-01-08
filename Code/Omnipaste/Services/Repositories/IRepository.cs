namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public interface IRepository<T>
        where T : BaseModel
    {
        IObservable<RepositoryOperation<T>> OperationObservable { get; }

        IObservable<RepositoryOperation<T>> Save(T item);

        IObservable<RepositoryOperation<T>> Delete(string id);

        IObservable<T> Get(string id);

        IObservable<IEnumerable<T>> GetAll();

        IObservable<IEnumerable<T>> GetAll(Func<T, bool> filter);
    }
}
