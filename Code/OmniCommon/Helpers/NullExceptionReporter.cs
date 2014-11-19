namespace OmniCommon.Helpers
{
    using System;

    public class NullExceptionReporter : IExceptionReporter
    {
        public void Report(Exception exception)
        {
        }

        public static NullExceptionReporter Instance
        {
            get
            {
                return new NullExceptionReporter();
            }
        }
    }
}