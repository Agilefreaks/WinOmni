namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Instrumentation;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using OmniCommon.Helpers;
    using Omnipaste.Models;

    public abstract class InMemoryRepository<T> : IRepository<T>
        where T : BaseModel
    {
        private readonly IList<T> _items;

        private readonly Subject<RepositoryOperation<T>> _subject;

        protected InMemoryRepository()
        {
            _subject = new Subject<RepositoryOperation<T>>();
            _items = new List<T>();
        }

        public IObservable<RepositoryOperation<T>> OperationObservable
        {
            get
            {
                return _subject;
            }
        }

        public IObservable<RepositoryOperation<T>> Save(T item)
        {
            return Async(() => SaveSynchronous(item));
        }

        public IObservable<T> Get(string id)
        {
            return Async(() => GetSynchronous(id));
        }

        public IObservable<IEnumerable<T>> GetAll()
        {
            return GetAll(_ => true);
        }

        public IObservable<IEnumerable<T>> GetAll(Func<T, bool> filter)
        {
            return Async(() => GetAllSynchronous(filter));
        }

        public IObservable<RepositoryOperation<T>> Delete(string id)
        {
            return Async(() => DeleteSynchronous(id));
        }

        public List<T> GetAllSynchronous(Func<T, bool> filter)
        {
            lock (_items)
            {
                return _items.Where(filter).ToList();
            }
        }

        public T GetSynchronous(string id)
        {
            lock (_items)
            {
                return _items.FirstOrDefault(item => item.UniqueId == id);
            }
        }

        public RepositoryOperation<T> SaveSynchronous(T item)
        {
            lock (_items)
            {
                _items.Add(item);
                var result = new RepositoryOperation<T>(RepositoryMethodEnum.Save, item);
                _subject.OnNext(result);

                return result;
            }
        }

        public RepositoryOperation<T> DeleteSynchronous(string id)
        {
            lock (_items)
            {
                var targetItem = _items.FirstOrDefault(item => item.UniqueId == id);
                if (targetItem == null)
                {
                    throw new InstanceNotFoundException();
                }

                _items.Remove(targetItem);
                var result = new RepositoryOperation<T>(RepositoryMethodEnum.Delete, targetItem);
                _subject.OnNext(result);

                return result;
            }
        }

        private IObservable<TResult> Async<TResult>(Func<TResult> funcToExecute)
        {
            return Observable.Start(funcToExecute, SchedulerProvider.Default);
        }
    }
}