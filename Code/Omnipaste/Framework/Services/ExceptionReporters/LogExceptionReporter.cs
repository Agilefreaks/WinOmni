namespace Omnipaste.Framework.Services.ExceptionReporters
{
    using System;
    using OmniCommon;
    using OmniCommon.Helpers;

    public class LogExceptionReporter : IExceptionReporter
    {
        public void Report(Exception exception)
        {
            SimpleLogger.Log(exception);
        }
    }
}