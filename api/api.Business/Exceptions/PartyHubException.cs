using System;

namespace Api.Business.Exceptions
{
    public class PartyHubException : Exception
    {
        public PartyHubException(string message) : base(message)
        {

        }
    }
}