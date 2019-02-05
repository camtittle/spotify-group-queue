using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Exceptions
{
    public class APIException : Exception
    {
        public APIException(string message) : base(message)
        {
        }
    }
}
