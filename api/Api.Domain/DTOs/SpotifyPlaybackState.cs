using Api.Domain.Entities;

namespace Api.Domain.DTOs
{
    public class SpotifyPlaybackState
    {
        public SpotifyDevice Device { get; set; }
        
        public int ProgressMillis { get; set; }
        
        public bool IsPlaying { get; set; }

        public SpotifyTrack Item { get; set; }

    }

    public class SpotifyTrack
    {
        public string Uri { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public int ProgressMillis { get; set; }

        public int DurationMillis { get; set; }
    }
}