using Microsoft.EntityFrameworkCore;

namespace api.Models
{
    [Owned]
    public class SpotifyDevice
    {
        public string DeviceId { get; set; }

        public string Name { get; set; }
    }
}