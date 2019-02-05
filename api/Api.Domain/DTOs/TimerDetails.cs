using Api.Domain.Entities;

namespace Api.Domain.DTOs
{
    public class TimerDetails
    {
        public Party Party { get; set; }

        public string TrackUri { get; set; }

        public int DelayMillis { get; set; }
    }
}