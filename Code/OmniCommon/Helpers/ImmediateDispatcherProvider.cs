namespace OmniCommon.Helpers
{
    public class ImmediateDispatcherProvider : IDispatcherProvider
    {
        private readonly ImmediateDispatcher _immediateDispatcher;

        public ImmediateDispatcherProvider()
        {
            _immediateDispatcher = new ImmediateDispatcher();
        }

        public IDispatcher Current
        {
            get
            {
                return _immediateDispatcher;
            }
        }

        public IDispatcher Application
        {
            get
            {
                return _immediateDispatcher;
            }
        }
    }
}