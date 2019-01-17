using Newtonsoft.Json;

namespace Spotify.Models
{
    public class PlaybackState
    {
        public Device Device { get; set; }

        [JsonProperty("progress_ms")]
        public int ProgressMillis { get; set; }

        [JsonProperty("is_playing")]
        public bool IsPlaying { get; set; }

        public Track Item { get; set; }
    }
}