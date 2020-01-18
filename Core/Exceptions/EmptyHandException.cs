using System;

namespace Core.Exceptions
{
    public class EmptyHandException : Exception
    {
        public EmptyHandException(string message) : base(message)
        {
        }
    }
}
