namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Instrumentation;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using OmniCommon.Helpers;

    public abstract class InMemoryRepository<T> : IRepository<T>
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

        public IObservable<T> Get(object id)
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

        public IObservable<RepositoryOperation<T>> Delete(object id)
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

        public T GetSynchronous(object id)
        {
            lock (_items)
            {
                return _items.FirstOrDefault(item => IsMatch(item, id));
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

        public RepositoryOperation<T> DeleteSynchronous(object id)
        {
            lock (_items)
            {
                var targetItem = _items.FirstOrDefault(item => IsMatch(item, id));
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

        protected abstract bool IsMatch(T item, object id);

        private IObservable<TResult> Async<TResult>(Func<TResult> funcToExecute)
        {
            return Observable.Start(funcToExecute, SchedulerProvider.Default);
        }
    }
}