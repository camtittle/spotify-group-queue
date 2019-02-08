using Api.Domain.Entities;

namespace Api.Domain.DTOs
{
    public class SpotifyPlaybackState
    {
        public SpotifyDevice Device { get; set; }
        
        public int ProgressMillis { get; set; }
        
        public bool IsPlaying { get; set; }

        public Track Item { get; set; }

    }
}