using System.ComponentModel.DataAnnotations;
using Api.Domain.DTOs;

namespace Api.DTOs
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
        public PartyStatus CurrentParty { get; set; }

        public RegisterResponse(string id, string username, string authToken, PartyStatus currentParty)
        {
            AuthToken = authToken;
            Username = username;
            Id = id;
            CurrentParty = currentParty;
        }
    }
}
