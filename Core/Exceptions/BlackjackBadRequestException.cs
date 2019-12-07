using System;

namespace Core.Exceptions
{
    public class BlackjackBadRequestException : Exception
    {
        public BlackjackBadRequestException(string message) : base(message)
        {
        }

        public BlackjackBadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
