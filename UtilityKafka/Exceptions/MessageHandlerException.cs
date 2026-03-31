using System;

namespace Utility.Kafka.Exceptions
{
    public class MessageHandlerException : ArgumentException
    {
        public MessageHandlerException() : base() { }

        public MessageHandlerException(string? message) : base(message) { }

        public MessageHandlerException(string? message, Exception? innerException) : base(message, innerException) { }

        public MessageHandlerException(string? message, string? paramName) : base(message, paramName) { }

        public MessageHandlerException(string? message, string? paramName, Exception? innerException) : base(message, paramName, innerException) { }
    }
}
