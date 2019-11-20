using System;

namespace BlackjackAPI.Exceptions
{
    public class DealEndedException : Exception
    {
        public DealEndedException(string message) : base(message)
        {
        }
    }
}