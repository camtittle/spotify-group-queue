using System;
using Api.Domain.Entities;

namespace Api.Domain.DTOs
{
    public class TimerSpecification
    {
        public TimerAction Action { get; set; }

        public Party Party { get; set; }

        public QueueItem QueueItem { get; set; }

        public DateTime ScheduledTimeUtc { get; set; }
    }

    public enum TimerAction
    {
        PlayQueueItem,
        DeactivatePlayback,
        CancelExistingTimer
    }
}