using System.Collections.Generic;

namespace api.Models
{
    public class PlaybackStatusUpdate
    {
        public string Uri { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public int DurationMillis { get; set; }

        public bool IsPlaying { get; set; }
        
        // Device fields only sent to party owner
        public string DeviceId { get; set; }

        public string DeviceName { get; set; }
    }
}