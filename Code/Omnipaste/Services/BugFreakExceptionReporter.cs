namespace Omnipaste.Services
{
    using System;
    using BugFreak;
    using OmniCommon.ExtensionMethods;

    public class BugFreakExceptionReporter : IExceptionReporter
    {
        public void Report(Exception exception)
        {
            ReportingService.Instance.BeginReport(exception);
        }
    }
}