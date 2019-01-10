using System.Collections.Generic;
using Newtonsoft.Json;

namespace Spotify.Models
{
    public class Track
    {
        public string Uri { get; set; }

        public List<ArtistSimplified> Artists { get; set; }

        public string Name { get; set; }

        [JsonProperty("duration_ms")]
        public int DurationMillis { get; set; }
    }
}