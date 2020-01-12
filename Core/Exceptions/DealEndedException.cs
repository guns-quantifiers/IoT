using System;

namespace Core.Exceptions
{
    public class DealEndedException : Exception
    {
        public DealEndedException(string message) : base(message)
        {
        }
    }
}