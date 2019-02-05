using System;

namespace api.Exceptions
{
    public class JoiningPartyException : Exception
    {
        public JoiningPartyException(string msg) : base(msg)
        {
        }
    }
}