using Microsoft.EntityFrameworkCore;

namespace Api.Domain.Entities
{
    [Owned]
    public class SpotifyDevice
    {
        public string DeviceId { get; set; }

        public string Name { get; set; }
    }
}