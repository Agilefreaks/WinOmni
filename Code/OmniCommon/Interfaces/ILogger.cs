namespace OmniCommon.Interfaces
{
    using System;

    public interface ILogger
    {
        void Info(string message);

        void Error(Exception exception);
    }
}