namespace OmniCommon.Helpers
{
    using System;
    using System.Windows.Threading;

    public class DispatcherWrapper : IDispatcher
    {
        private readonly Dispatcher _dispatcher;

        private DispatcherWrapper(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public static DispatcherWrapper FromCurrent()
        {
            return FromGiven(Dispatcher.CurrentDispatcher);
        }

        public void Dispatch(Delegate method, params object[] arguments)
        {
            _dispatcher.BeginInvoke(method, arguments);
        }

        public static DispatcherWrapper FromGiven(Dispatcher dispatcher)
        {
            return new DispatcherWrapper(dispatcher);
        }
    }
}