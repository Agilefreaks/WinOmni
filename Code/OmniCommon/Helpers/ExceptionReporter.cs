namespace OmniCommon.Helpers
{
    public class ExceptionReporter
    {
        private static IExceptionReporter _instance;

        public static IExceptionReporter Instance
        {
            get
            {
                return _instance ?? NullExceptionReporter.Instance;
            }
            set
            {
                _instance = value;
            }
        }
    }
}