namespace OmniCommon.Logging
{
    using System;
    using OmniCommon.Interfaces;

    public class NullLoger : ILogger
    {
        public void Info(string message)
        {
        }

        public void Error(Exception exception)
        {
        }
    }
}