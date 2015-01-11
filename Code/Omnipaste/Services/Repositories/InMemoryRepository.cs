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
                        _items.Add(item);
                        var result = new RepositoryOperation<T>(RepositoryMethodEnum.Save, item);
                        _subject.OnNext(result);

                        return result;
                    },
                SchedulerProvider.Default);
        }

        public IObservable<IEnumerable<T>> GetAll()
        {
            return GetAll(_ => true);
        }

        public IObservable<IEnumerable<T>> GetAll(Func<T, bool> include)
        {
            return Observable.Start(() => _items.Where(include).ToList(), SchedulerProvider.Default);
        }

        public IObservable<RepositoryOperation<T>> Delete(object id)
        {
            return Observable.Start(
                () =>
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
                    },
                SchedulerProvider.Default);
        }

        protected abstract bool IsMatch(T item, object id);
    }
}