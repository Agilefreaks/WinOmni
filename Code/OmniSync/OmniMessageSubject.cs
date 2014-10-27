namespace OmniSync
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Disposables;
    using System.Reactive.Subjects;
    using Castle.Core.Internal;
    using OmniCommon.Models;

    public class OmniMessageSubject : ISubject<OmniMessage>
    {
        #region Fields

        private readonly IList<IObserver<OmniMessage>> _observers = new List<IObserver<OmniMessage>>();

        #endregion

        #region Public Methods and Operators

        public void OnCompleted()
        {
            _observers.ForEach(observer => observer.OnCompleted());
        }

        public void OnError(Exception error)
        {
            _observers.ForEach(observer => observer.OnError(error));
        }

        public void OnNext(OmniMessage value)
        {
            _observers.ForEach(observer => observer.OnNext(value));
        }

        public IDisposable Subscribe(IObserver<OmniMessage> observer)
        {
            var disposable = Disposable.Create(() => RemoveObserver(observer));

            lock (_observers)
            {
                _observers.Add(observer);                
            }

            return disposable;
        }

        #endregion

        #region Methods

        private void RemoveObserver(IObserver<OmniMessage> observer)
        {
            lock (_observers)
            {
                if (_observers.Contains(observer))
                {
                    _observers.Remove(observer);
                }
            }
        }

        #endregion
    }
}