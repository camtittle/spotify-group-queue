using System.ComponentModel.DataAnnotations;

namespace Api.Domain.DTOs
{
    public class AddTrackToQueueRequest
    {
        [Required]
        public string SpotifyUri { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Artist { get; set; }

        [Required]
        public long DurationMillis { get; set; }
    }
}