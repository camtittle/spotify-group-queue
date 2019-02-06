using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    public class SetSpotifyDeviceRequest
    {
        [Required]
        public string DeviceName { get; set; }

        [Required]
        public string DeviceId { get; set; }
    }
}