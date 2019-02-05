using System.Linq;
using System.Threading.Tasks;
using Api.Business.Exceptions;
using Api.Domain.DTOs;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;

namespace Api.Business.Services
{
    public class QueueService : IQueueService
    {
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly IPartyRepository _partyRepository;

        public QueueService(IQueueItemRepository queueItemRepository, IPartyRepository partyRepository)
        {
            _queueItemRepository = queueItemRepository;
            _partyRepository = partyRepository;
        }

        public async Task<QueueItem> AddQueueItem(User user, AddTrackToQueueRequest request)
        {
            if (!user.IsOwner && !user.IsMember)
            {
                throw new PartyQueueException("Cannot add request to queue - not a member of a party");
            }

            var party = await _partyRepository.GetWithAllProperties(user.GetCurrentParty());
            if (party == null)
            {
                throw new PartyQueueException("Cannot add request to queue - party not found");
            }

            var newIndex = party.QueueItems.Count > 0 ? party.QueueItems.Max(x => x.Index) + 1 : 1;

            // TODO: limit number of tracks in queue per user
            var queueItem = new QueueItem()
            {
                AddedByUser = user,
                ForParty = party,
                SpotifyUri = request.SpotifyUri,
                Title = request.Title,
                Artist = request.Artist,
                DurationMillis = request.DurationMillis,
                Index = newIndex
            };

            await _queueItemRepository.Add(queueItem);

            return queueItem;
        }

        public async Task RemoveQueueItem(User user, string queueItemId)
        {
            if (!user.IsOwner)
            {
                throw new PartyQueueException("Cannot remove from queue - not owner of party");
            }

            var party = await _partyRepository.GetWithAllProperties(user.GetCurrentParty());
            if (party == null)
            {
                throw new PartyQueueException("Cannot remove track from queue - party not found");
            }

            var queueItem = await _queueItemRepository.Get(queueItemId);
            if (queueItem == null)
            {
                throw new PartyQueueException("Cannot remove track from queue - queue item with ID not found");
            }

            await _queueItemRepository.Delete(queueItem);
        }

    }
}