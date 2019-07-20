using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Services
{
    public interface IQueueService
    {
        Task<QueueItem> AddQueueItem(User user, AddTrackToQueueRequest request, bool notifyClients = false);
        Task RemoveQueueItem(string queueItemId);
        Task<QueueItem> GetNextQueueItem(Party party);
    }
}