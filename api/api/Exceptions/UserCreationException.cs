using System;

namespace api.Exceptions
{
    public class UserCreationException : Exception
    {
        public UserCreationException(string msg) : base(msg)
        {
        }
    }
}