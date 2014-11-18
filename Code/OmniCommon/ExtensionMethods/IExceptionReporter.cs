namespace OmniCommon.ExtensionMethods
{
    using System;

    public interface IExceptionReporter
    {
        void Report(Exception exception);
    }
}