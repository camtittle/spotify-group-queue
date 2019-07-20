using System.Linq;
using System.Threading.Tasks;
using Api.Business.Exceptions;
using Api.Domain.DTOs;
using Api.Domain.Entities;
using Api.Domain.Enums;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;

namespace Api.Business.Services
{
    public class QueueService : IQueueService
    {
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly IPartyRepository _partyRepository;

        private readonly IRealTimeService _realTimeService;
        private readonly ITimerQueueService _timerQueueService;

        public QueueService(IQueueItemRepository queueItemRepository, IPartyRepository partyRepository, IRealTimeService realTimeService, ITimerQueueService timerQueueService)
        {
            _queueItemRepository = queueItemRepository;
            _partyRepository = partyRepository;
            _realTimeService = realTimeService;
            _timerQueueService = timerQueueService;
        }

        public async Task<QueueItem> AddQueueItem(User user, AddTrackToQueueRequest request, bool notifyClients = false)
        {
            if (!user.IsOwner && !user.IsMember)
            {
                throw new PartyQueueException("Cannot add request to queue - not a member of a party");
            }

            var party = await _partyRepository.GetWithAllProperties(user.GetActiveParty());
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

            // If this is the first item on the queue, set up a timer
            if (party.Playback != Playback.NotActive && party.QueueItems.Count == 1)
            {
                _timerQueueService.Enqueue(new TimerSpecification
                {
                    Action = TimerAction.PlayQueueItem,
                    Party = party,
                    QueueItem = queueItem,
                    ScheduledTimeUtc = party.CurrentTrack.ExpectedFinishTime
                });
            }

            if (notifyClients)
            {
                await _realTimeService.SendPartyStatusUpdate(party);
            }

            return queueItem;
        }

        public async Task RemoveQueueItem(string queueItemId)
        {
            var queueItem = await _queueItemRepository.Get(queueItemId);
            await _queueItemRepository.Delete(queueItem);

            await _realTimeService.SendPartyStatusUpdate(queueItem.ForParty);
        }

        public async Task<QueueItem> GetNextQueueItem(Party party)
        {
            if (party.QueueItems == null)
            {
                party = await _partyRepository.GetWithAllProperties(party);
            }

            if (party.QueueItems.Count > 0)
            {
                return party.QueueItems.OrderBy(x => x.Index).First();
            }

            return null;
        }
    }
}