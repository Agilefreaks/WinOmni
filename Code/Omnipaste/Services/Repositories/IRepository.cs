namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;
    using OmniUI.Entities;

    public interface IReporsitory
    {
        IObservable<RepositoryOperation<T>> GetOperationObservable<T>() where T : Entity;

        IObservable<RepositoryOperation<T>> Save<T>(T item) where T : Entity;

        IObservable<RepositoryOperation<T>> Delete<T>(string id) where T : Entity;

        IObservable<T> Get<T>(string id) where T : Entity;

        IObservable<T> Get<T>(Func<T, bool> match) where T : Entity;

        IObservable<IEnumerable<T>> GetAll<T>() where T : Entity;

        IObservable<IEnumerable<T>> GetAll<T>(Func<T, bool> filter) where T : Entity;
    }

    public interface IRepository<T>
        where T : Entity
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
