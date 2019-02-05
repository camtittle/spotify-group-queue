using System;

namespace Api.Business.Exceptions
{
    public class UserCreationException : Exception
    {
        public UserCreationException(string msg) : base(msg)
        {
        }
    }
}