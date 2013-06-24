namespace Omnipaste
{
    using System;
    using System.Globalization;
    using Common.Logging;
    using Common.Logging.Simple;

    public class SimpleDefferingLogger : AbstractSimpleLogger
    {
        private readonly Action<string> _logAction;

        public SimpleDefferingLogger(Action<string> logAction)
            : base("BallongLogger", LogLevel.All, false, false, false, DateTimeFormatInfo.InvariantInfo.RFC1123Pattern)
        {
            _logAction = logAction;
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            _logAction(message.ToString());
        }
    }
}