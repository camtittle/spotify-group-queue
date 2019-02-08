using System.Collections.Concurrent;
using Api.Domain.DTOs;
using Api.Domain.Interfaces.Services;

namespace Api.Business.Services
{
    public class TimerQueueService : ITimerQueueService
    {
        private readonly BlockingCollection<TimerSpecification> _items;

        public TimerQueueService()
        {
            _items = new BlockingCollection<TimerSpecification>();
        }

        public void Enqueue(TimerSpecification specification)
        {
            _items.Add(specification);
        }

        public TimerSpecification Dequeue()
        {
            return _items.Take();
        }
    }
}