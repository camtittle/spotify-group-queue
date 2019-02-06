using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    public class SpotifyAuthorizationRequest
    {
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string Code { get; set; }
    }
}