namespace OmniDebug.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;

    public abstract class ResourceWrapperBase<T>
        where T : class
    {
        private readonly IResource<T> _originalResource;

        private readonly Dictionary<string, T> _resourcesById;

        private T _resourceToPush;

        protected ResourceWrapperBase(IResource<T> originalResource)
        {
            _originalResource = originalResource;
            _resourcesById = new Dictionary<string, T>();
        }

        public IObservable<T> Last()
        {
            IObservable<T> observable;
            if (_resourceToPush != null)
            {
                observable = Observable.Return(_resourceToPush, SchedulerProvider.Default);
                _resourceToPush = null;
            }
            else
            {
                observable = _originalResource.Last();
            }

            return observable;
        }

        public IObservable<T> Get(string id)
        {
            return _resourcesById.ContainsKey(id)
                       ? Observable.Return(_resourcesById[id], SchedulerProvider.Default)
                       : _originalResource.Get(id);
        }

        public void MockLast(T resource)
        {
            _resourceToPush = resource;
        }

        public void MockGet(string id, T item)
        {
            _resourcesById[id] = item;
        }
    }
}