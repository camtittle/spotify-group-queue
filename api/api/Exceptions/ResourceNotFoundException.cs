using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace api.Exceptions
{
    public class ResourceNotFoundException : APIException
    {
        public ResourceNotFoundException(string message) : base(message)
        {
        }
    }
}
