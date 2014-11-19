namespace OmniCommon.Helpers
{
    using System;

    public interface IExceptionReporter
    {
        void Report(Exception exception);
    }
}