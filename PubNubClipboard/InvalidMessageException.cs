namespace PubNubClipboard
{
    using System;

    public class InvalidMessageException : Exception
    {
        public const string DefaultMessage = "Invalid message detected";

        public InvalidMessageException()
            : base(DefaultMessage)
        {
        }

        public InvalidMessageException(string message)
            : base(message)
        {
        }

        public InvalidMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}