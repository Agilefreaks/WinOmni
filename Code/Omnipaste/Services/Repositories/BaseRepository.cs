namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Omnipaste.Models;

    public abstract class BaseRepositroy : IReporsitory
    {
        protected readonly Subject<RepositoryOperation> Subject = new Subject<RepositoryOperation>();

        public IObservable<RepositoryOperation<T>> GetOperationObservable<T>() where T : BaseModel
        {
            return Subject.OfType<RepositoryOperation<T>>();
        }

        public abstract IObservable<RepositoryOperation<T>> Save<T>(T item) where T : BaseModel;

        public abstract IObservable<RepositoryOperation<T>> Delete<T>(string id) where T : BaseModel;

        public abstract IObservable<T> Get<T>(string id) where T : BaseModel;

        public abstract IObservable<T> Get<T>(Func<T, bool> match) where T : BaseModel;

        public abstract IObservable<IEnumerable<T>> GetAll<T>() where T : BaseModel;

        public abstract IObservable<IEnumerable<T>> GetAll<T>(Func<T, bool> filter) where T : BaseModel;
    }

    public abstract class BaseRepository<T> : IRepository<T>
        where T : BaseModel
    {
        protected readonly Subject<RepositoryOperation<T>> Subject = new Subject<RepositoryOperation<T>>();

        #region IRepository<T> Members

        public virtual IObservable<RepositoryOperation<T>> GetOperationObservable()
        {
            return Subject;
        }

        public abstract IObservable<RepositoryOperation<T>> Save(T item);

        public abstract IObservable<RepositoryOperation<T>> Delete(string id);

        public abstract IObservable<T> Get(string id);

        public abstract IObservable<T> Get(Func<T, bool> match);

        public abstract IObservable<IEnumerable<T>> GetAll();

        public abstract IObservable<IEnumerable<T>> GetAll(Func<T, bool> filter);

        #endregion
    }
}