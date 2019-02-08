using System.Collections.Concurrent;
using Api.Domain.DTOs;
using Api.Domain.Interfaces.Services;

namespace Api.Business.Services
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