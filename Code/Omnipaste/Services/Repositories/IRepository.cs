namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public interface IReporsitory
    {
        IObservable<RepositoryOperation<T>> GetOperationObservable<T>() where T : BaseModel;

        IObservable<RepositoryOperation<T>> Save<T>(T item) where T : BaseModel;

        IObservable<RepositoryOperation<T>> Delete<T>(string id) where T : BaseModel;

        IObservable<T> Get<T>(string id) where T : BaseModel;

        IObservable<T> Get<T>(Func<T, bool> match) where T : BaseModel;

        IObservable<IEnumerable<T>> GetAll<T>() where T : BaseModel;

        IObservable<IEnumerable<T>> GetAll<T>(Func<T, bool> filter) where T : BaseModel;
    }

    public interface IRepository<T>
        where T : BaseModel
    {
        IObservable<RepositoryOperation<T>> GetOperationObservable();

        IObservable<RepositoryOperation<T>> Save(T item);

        IObservable<RepositoryOperation<T>> Delete(string id);

        IObservable<T> Get(string id);

        IObservable<T> Get(Func<T, bool> match);

        IObservable<IEnumerable<T>> GetAll();

        IObservable<IEnumerable<T>> GetAll(Func<T, bool> filter);
    }
}
