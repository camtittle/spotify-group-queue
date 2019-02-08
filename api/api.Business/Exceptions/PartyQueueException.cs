using System;

namespace Api.Business.Exceptions
{
    public class PartyQueueException : Exception
    {
        public PartyQueueException(string message) : base(message)
        {

        }
    }
}