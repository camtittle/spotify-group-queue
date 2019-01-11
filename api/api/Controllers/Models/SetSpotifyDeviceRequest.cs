using Microsoft.Build.Framework;

namespace api.Controllers.Models
{
    public class SetSpotifyDeviceRequest
    {
        [Required]
        public string DeviceName { get; set; }

        [Required]
        public string DeviceId { get; set; }
    }
}