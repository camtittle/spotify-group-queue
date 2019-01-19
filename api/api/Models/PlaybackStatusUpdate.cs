using System.Collections.Generic;
using api.Enums;

namespace api.Models
{
    public class PlaybackStatusUpdate
    {
        public string Uri { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public int DurationMillis { get; set; }
        
        // Device fields only sent to party owner
        public string DeviceId { get; set; }

        public string DeviceName { get; set; }

        // Whether initial queue playback has been invoked
        public Playback PlaybackState { get; set; }
    }
}