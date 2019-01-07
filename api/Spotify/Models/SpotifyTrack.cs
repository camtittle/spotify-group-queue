using System.Collections.Generic;
using Newtonsoft.Json;

namespace Spotify.Models
{
    public class SpotifyTrack
    {
        public string Uri;

        public List<SpotifyArtistSimplified> Artists;
        
        public string Name;

        [JsonProperty("duration_ms")]
        public int DurationMillis;
    }
}