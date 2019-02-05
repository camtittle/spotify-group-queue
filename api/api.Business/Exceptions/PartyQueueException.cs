using System;

namespace api.Exceptions
{
    public class PartyQueueException : Exception
    {
        public PartyQueueException(string message) : base(message)
        {

        }
    }
}