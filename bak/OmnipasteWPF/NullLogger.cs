namespace OmnipasteWPF
{
    using System;
    using Cinch;

    public class NullLogger : ILogger
    {
        public void Error(Exception exception)
        {
        }

        public void Error(object obj)
        {
        }

        public void Error(string message)
        {
        }

        public void Error(Exception exception, string message)
        {
        }

        public void Error(string format, params object[] args)
        {
        }

        public void Error(Exception exception, string format, params object[] args)
        {
        }

        public void Error(IFormatProvider provider, string format, params object[] args)
        {
        }

        public void Error(Exception exception, string format, IFormatProvider provider, params object[] args)
        {
        }
    }
}