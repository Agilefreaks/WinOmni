namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Caliburn.Micro;
    using OmniCommon.Helpers;
    using Omnipaste.Models;

    public abstract class InMemoryRepository<T> : IRepository<T>
        where T : BaseModel
    {
        private readonly IDictionary<string, T> _items;

        private readonly Subject<RepositoryOperation<T>> _subject;

        protected InMemoryRepository()
        {
            _subject = new Subject<RepositoryOperation<T>>();
            _items = new Dictionary<string, T>();
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

        public IObservable<T> Get(Func<T, bool> match)
        {
            return Async(() => GetSynchronous(match));
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
                return _items.Values.Where(filter).ToList();
            }
        }

        public T GetSynchronous(string id)
        {
            lock (_items)
            {
                return _items.GetValueOrDefault(id);
            }
        }

        public T GetSynchronous(Func<T, bool> match)
        {
            lock (_items)
            {
                return _items.Values.First(match);
            }
        }

        public RepositoryOperation<T> SaveSynchronous(T item)
        {
            lock (_items)
            {
                var value = _items.GetValueOrDefault(item.UniqueId);
                var operation = value == null ? RepositoryMethodEnum.Create : RepositoryMethodEnum.Update;
                var result = new RepositoryOperation<T>(operation, item);
                _items[item.UniqueId] = item;
                _subject.OnNext(result);

                return result;
            }
        }

        public RepositoryOperation<T> DeleteSynchronous(string id)
        {
            lock (_items)
            {
                var targetItem = _items.GetValueOrDefault(id);
                _items.Remove(id);
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