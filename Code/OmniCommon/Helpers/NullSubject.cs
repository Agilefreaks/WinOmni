namespace OmniCommon.Helpers
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Subjects;

    public class NullSubject<T> : ISubject<T>
    {
        public void OnNext(T value)
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Disposable.Empty;
        }
    }
}