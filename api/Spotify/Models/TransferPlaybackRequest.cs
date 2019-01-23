using System.Collections.Generic;
using Newtonsoft.Json;

namespace Spotify.Models
{
    public class TransferPlaybackRequest
    {
        [JsonProperty("device_ids")]
        public string[] DeviceIds { get; set; }
        public bool Play { get; set; }
    }
}