using System.Collections.Concurrent;
using System.Threading.Tasks;
using api.Models;

namespace api.Services.Interfaces
{
    public interface ITimerQueueService
    {
        void Enqueue(TimerDetails details);

        TimerDetails Dequeue();
    }
}