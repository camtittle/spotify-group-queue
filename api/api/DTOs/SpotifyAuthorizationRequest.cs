using System.ComponentModel.DataAnnotations;

namespace api.Controllers.Models
{
    public class SpotifyAuthorizationRequest
    {
        [Microsoft.Build.Framework.Required]
        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string Code { get; set; }
    }
}