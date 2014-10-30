namespace OmniDebug.Services
{
    using System;
    using System.Reactive.Linq;
    using OmniCommon.Interfaces;

    public abstract class ResourceWrapperBase<T>
        where T : class
    {
        private readonly IResource<T> _originalResource;

        private T _resourceToPush;

        protected ResourceWrapperBase(IResource<T> originalResource)
        {
            _originalResource = originalResource;
        }

        public IObservable<T> Last()
        {
            IObservable<T> observable;
            if (_resourceToPush != null)
            {
                observable = Observable.Return(_resourceToPush);
                _resourceToPush = null;
            }
            else
            {
                observable = _originalResource.Last();
            }

            return observable;
        }

        public void MockLast(T resource)
        {
            _resourceToPush = resource;
        }
    }
}