namespace OmniCommon.Handlers
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Models;

    public abstract class ResourceHandler<TEntity> : IResourceHandler<TEntity>
    {
        private IDisposable _subscription;

        protected readonly Subject<TEntity> InnerSubject;

        protected ResourceHandler()
        {
            InnerSubject = new Subject<TEntity>();
        }

        public abstract string HandledMessageType { get; }

        public virtual void Start(IObservable<OmniMessage> omniMessageObservable)
        {
            Stop();
            _subscription = omniMessageObservable.Where(m => m.Type == HandledMessageType).Subscribe(this);
        }

        public virtual void Stop()
        {
            Dispose();
        }

        public virtual void OnNext(OmniMessage value)
        {
            CreateResult(value).RunToCompletion(result => InnerSubject.OnNext(result));
        }

        public void OnError(Exception error)
        {
        }

        public virtual void OnCompleted()
        {
        }

        public IDisposable Subscribe(IObserver<TEntity> observer)
        {
            return InnerSubject.Subscribe(observer);
        }

        public virtual void Dispose()
        {
            try
            {
                if (_subscription != null)
                {
                    _subscription.Dispose();
                }
            }
            catch (Exception exception)
            {
                ExceptionReporter.Instance.Report(exception);
            }
        }

        protected abstract IObservable<TEntity> CreateResult(OmniMessage value);
    }
}
