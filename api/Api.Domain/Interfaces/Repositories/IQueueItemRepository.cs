using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Repositories
{
    public interface IQueueItemRepository
    {
        Task Add(QueueItem queueItem);
        Task<QueueItem> Get(string id);
        Task Delete(QueueItem queueItem);
    }
}