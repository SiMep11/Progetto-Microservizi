using System;

namespace Utility.Kafka.Exceptions
{
    internal class MessageException : ArgumentException
    {
        public MessageException() : base() { }

        public MessageException(string? message) : base(message) { }

        public MessageException(string? message, Exception? innerException) : base(message, innerException) { }

        public MessageException(string? message, string? paramName) : base(message, paramName) { }

        public MessageException(string? message, string? paramName, Exception? innerException) : base(message, paramName, innerException) { }
    }
}