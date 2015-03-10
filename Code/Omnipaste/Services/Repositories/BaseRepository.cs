namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Subjects;
    using Omnipaste.Models;

    public abstract class BaseRepository<T> : IRepository<T>
        where T : BaseModel
    {
        protected readonly Subject<RepositoryOperation<T>> _subject =  new Subject<RepositoryOperation<T>>();

        #region IRepository<T> Members

        public IObservable<RepositoryOperation<T>> OperationObservable
        {
            get
            {
                return _subject;
            }
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