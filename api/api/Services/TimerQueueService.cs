using System.Collections.Concurrent;
using System.Threading.Tasks;
using api.Models;
using api.Services.Interfaces;

namespace api.Services
{
    public class TimerQueueService : ITimerQueueService
    {
        private readonly BlockingCollection<TimerDetails> _items;

        public TimerQueueService()
        {
            _items = new BlockingCollection<TimerDetails>();
        }

        public void Enqueue(TimerDetails details)
        {
            _items.Add(details);
        }

        public TimerDetails Dequeue()
        {
            return _items.Take();
        }
    }
}