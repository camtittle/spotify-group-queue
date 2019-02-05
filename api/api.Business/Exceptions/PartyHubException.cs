using System;

namespace api.Exceptions
{
    public class PartyHubException : Exception
    {
        public PartyHubException(string message) : base(message)
        {

        }
    }
}