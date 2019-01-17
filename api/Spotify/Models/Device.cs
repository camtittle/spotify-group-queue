using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Spotify.Models
{
    public class Device
    {
        public string Id { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        [JsonProperty("volume_percent")]
        public int VolumePercent { get; set; }
    }
}