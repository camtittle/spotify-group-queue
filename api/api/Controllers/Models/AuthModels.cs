using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Controllers.Models
{
    public class TokenRequest
    {
        [Required] public string Username;
        [Required] public string DeveloperPassword;
    }

    public class RegisterRequest
    {
        [Required]
        [StringLength(30)]
        public string Username;
    }

    public class RegisterResponse
    {
        [Required] public string AuthToken;
        [Required] public string Username;
        [Required] public string Id;
        public CurrentParty CurrentParty;

        public RegisterResponse(string id, string username, string authToken, CurrentParty currentParty)
        {
            AuthToken = authToken;
            Username = username;
            Id = id;
            CurrentParty = currentParty;
        }
    }
}
