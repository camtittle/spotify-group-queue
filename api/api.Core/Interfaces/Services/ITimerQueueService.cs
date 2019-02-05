using api.Domain.DTOs;

namespace api.Domain.Interfaces.Services
{
    public interface ITimerQueueService
    {
        void Enqueue(TimerDetails details);

        TimerDetails Dequeue();
    }
}