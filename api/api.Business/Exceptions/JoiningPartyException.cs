using System;

namespace Api.Business.Exceptions
{
    public class JoiningPartyException : Exception
    {
        public JoiningPartyException(string msg) : base(msg)
        {
        }
    }
}