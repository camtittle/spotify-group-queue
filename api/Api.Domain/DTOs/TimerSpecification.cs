using Api.Domain.Entities;

namespace Api.Domain.DTOs
{
    public class TimerSpecification
    {
        public TimerInstruction Instruction { get; set; }

        public Party Party { get; set; }

        public QueueItem QueueItem { get; set; }

        public long DelayMillis { get; set; }
    }

    public enum TimerInstruction
    {
        PlayQueueItem,
        DeactivatePlayback,
        CancelExistingTimer
    }
}