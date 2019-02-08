using System;
using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Repositories;
using Api.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories
{
    public class QueueItemRepository : IQueueItemRepository
    {
        private readonly SpotifyAppContext _context;

        public QueueItemRepository(SpotifyAppContext context)
        {
            _context = context;
        }

        public async Task Add(QueueItem queueItem)
        {
            if (queueItem == null)
            {
                throw new ArgumentNullException();
            }

           _context.QueueItems.Add(queueItem);
           await _context.SaveChangesAsync();
        }

        public async Task<QueueItem> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException();
            }

            return await _context.QueueItems
                .Include(q => q.AddedByUser)
                .Include(q => q.ForParty)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task Delete(QueueItem queueItem)
        {
            if (queueItem == null)
            {
                throw new ArgumentNullException();
            }

            _context.Remove(queueItem);
            await _context.SaveChangesAsync();
        }
    }
}