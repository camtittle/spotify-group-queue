using api.Domain.Entities;

namespace api.Domain.DTOs
{
    public class TimerDetails
    {
        public Party Party { get; set; }

        public string TrackUri { get; set; }

        public int DelayMillis { get; set; }
    }
}