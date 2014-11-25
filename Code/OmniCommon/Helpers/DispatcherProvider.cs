namespace OmniCommon.Helpers
{
    public class DispatcherProvider
    {
        private static IDispatcherProvider _instance;

        public static IDispatcherProvider Instance
        {
            get
            {
                return _instance ?? (_instance = new ImmediateDispatcherProvider());
            }
            set
            {
                _instance = value;
            }
        }

        public static IDispatcher Current
        {
            get
            {
                return Instance.Current;
            }
        }

        public static IDispatcher Application
        {
            get
            {
                return Instance.Application;
            }
        }
    }
}