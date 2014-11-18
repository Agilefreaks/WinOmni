namespace OmniCommon.Helpers
{
    public class DispatcherProvider
    {
        private static IDispatcher _currentDispatcher;

        public static IDispatcher Current
        {
            get
            {
                return _currentDispatcher ?? DispatcherWrapper.FromCurrent();
            }
            set
            {
                _currentDispatcher = value;
            }
        }
    }
}