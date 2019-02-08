using Api.Domain.Entities;

namespace Api.Domain.DTOs
{
    public class TimerDetails
    {
        public Party Party { get; set; }

        public QueueItem QueueItem { get; set; }

        public long DelayMillis { get; set; }
    }
}