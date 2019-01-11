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
        [Required] public string Username { get; set; }
        [Required] public string DeveloperPassword { get; set; }
    }

    public class RegisterRequest
    {
        [Required]
        [StringLength(30)]
        public string Username;
    }

    public class RegisterResponse
    {
        [Required] public string AuthToken { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string Id { get; set; }
        public CurrentParty CurrentParty { get; set; }

        public RegisterResponse(string id, string username, string authToken, CurrentParty currentParty)
        {
            AuthToken = authToken;
            Username = username;
            Id = id;
            CurrentParty = currentParty;
        }
    }
}
