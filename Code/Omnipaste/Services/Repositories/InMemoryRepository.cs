namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using OmniUI.Entities;

    public abstract class InMemoryRepository<T> : BaseRepository<T>
        where T : Entity
    {
        private readonly ConcurrentDictionary<string, T> _items;

        protected InMemoryRepository()
        {
            _items = new ConcurrentDictionary<string, T>();
        }

        public override IObservable<RepositoryOperation<T>> Save(T item)
        {
            return Async(() => SaveSynchronous(item));
        }

        public override IObservable<T> Get(string id)
        {
            return Async(() => GetSynchronous(id));
        }

        public override IObservable<T> Get(Func<T, bool> match)
        {
            return Async(() => GetSynchronous(match));
        }

        public override IObservable<IEnumerable<T>> GetAll()
        {
            return GetAll(_ => true);
        }

        public override IObservable<IEnumerable<T>> GetAll(Func<T, bool> filter)
        {
            return Async(() => GetAllSynchronous(filter));
        }

        public override IObservable<RepositoryOperation<T>> Delete(string id)
        {
            return Async(() => DeleteSynchronous(id));
        }

        public List<T> GetAllSynchronous(Func<T, bool> filter)
        {
            return _items.Values.Where(filter).ToList();
        }

        public T GetSynchronous(string id)
        {
            return _items.GetValueOrDefault(id);
        }

        public T GetSynchronous(Func<T, bool> match)
        {
            return _items.Values.First(match);
        }

        public RepositoryOperation<T> SaveSynchronous(T item)
        {
            var result = new RepositoryOperation<T>(RepositoryMethodEnum.Changed, item);
            _items.AddOrUpdate(item.UniqueId, item, (key, existingValue) => item);
            Subject.OnNext(result);

            return result;
        }

        public RepositoryOperation<T> DeleteSynchronous(string id)
        {
            var targetItem = _items.GetValueOrDefault(id);
            T removedValue;
            _items.TryRemove(id, out removedValue);
            var result = new RepositoryOperation<T>(RepositoryMethodEnum.Delete, targetItem);
            Subject.OnNext(result);

            return result;
        }

        private IObservable<TResult> Async<TResult>(Func<TResult> funcToExecute)
        {
            return Observable.Start(funcToExecute, SchedulerProvider.Default);
        }
    }
}