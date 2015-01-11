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
            return Observable.Start(
                () =>
                    {
                        lock (_items)
                        {
                            _items.Add(item);
                            var result = new RepositoryOperation<T>(RepositoryMethodEnum.Save, item);
                            _subject.OnNext(result);

                            return result;
                        }
                    },
                SchedulerProvider.Default);
        }

        public IObservable<T> Get(object id)
        {
            return Observable.Start(
                () =>
                    {
                        lock (_items)
                        {
                            return _items.FirstOrDefault(item => IsMatch(item, id));
                        }
                    },
                SchedulerProvider.Default);
        }

        public IObservable<IEnumerable<T>> GetAll()
        {
            return GetAll(_ => true);
        }

        public IObservable<IEnumerable<T>> GetAll(Func<T, bool> include)
        {
            lock (_items)
            {
                return Observable.Start(
                    () =>
                        {
                            lock (_items)
                            {
                                return _items.Where(include).ToList();
                            }
                        },
                    SchedulerProvider.Default);
            }
        }

        public IObservable<RepositoryOperation<T>> Delete(object id)
        {
            return Observable.Start(
                () =>
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

                    },
                SchedulerProvider.Default);
        }

        protected abstract bool IsMatch(T item, object id);
    }
}