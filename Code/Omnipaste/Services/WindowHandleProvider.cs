namespace Omnipaste.Services
{
    using System;
    using System.Reactive.Subjects;
    using OmniCommon.Interfaces;

    public class WindowHandleProvider : IWindowHandleProvider
    {
        public class AlreadyInitializedException : Exception
        {
            public AlreadyInitializedException()
                : base("Handle has been already initialized")
            {
            }
        }

        private readonly ReplaySubject<IntPtr> _replaySubject;

        private IntPtr? _handle;

        public WindowHandleProvider()
        {
            _replaySubject = new ReplaySubject<IntPtr>(1);
        }

        public void SetHandle(IntPtr windowHandle)
        {
            if (_handle == null)
            {
                _handle = windowHandle;
                _replaySubject.OnNext(windowHandle);
                _replaySubject.OnCompleted();
            }
            else
            {
                throw new AlreadyInitializedException();
            }
        }

        public IDisposable Subscribe(IObserver<IntPtr> observer)
        {
            return _replaySubject.Subscribe(observer);
        }
    }
}