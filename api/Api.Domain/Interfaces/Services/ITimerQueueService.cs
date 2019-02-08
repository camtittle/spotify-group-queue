using Api.Domain.DTOs;

namespace Api.Domain.Interfaces.Services
{
    public interface ITimerQueueService
    {
        void Enqueue(TimerSpecification specification);

        TimerSpecification Dequeue();
    }
}