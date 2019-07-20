using System;
using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Entities;
using Api.Domain.Enums;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;

namespace Api.Business.Services
{
    public class PlaybackService : IPlaybackService
    {
        private readonly IPartyRepository _partyRepository;
        private readonly IQueueItemRepository _queueItemRepository;

        private readonly IQueueService _queueService;
        private readonly ISpotifyService _spotifyService;
        private readonly ITimerQueueService _timerQueueService;
        private readonly IRealTimeService _realTimeService;

        public PlaybackService(IPartyRepository partyRepository, IQueueItemRepository queueItemRepository, IQueueService queueService, ISpotifyService spotifyService, ITimerQueueService timerQueueService, IRealTimeService realTimeService)
        {
            _partyRepository = partyRepository;
            _queueItemRepository = queueItemRepository;
            _queueService = queueService;
            _spotifyService = spotifyService;
            _timerQueueService = timerQueueService;
            _realTimeService = realTimeService;
        }

        public async Task UpdatePlaybackState(Party party, SpotifyPlaybackState state, User[] exemptUsers = null)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            party = await _partyRepository.GetWithAllProperties(party);

            if (state == null)
            {
                party.CurrentTrack = new Track();

                party.Playback = Playback.NotActive;

                party.Owner.CurrentDevice = new SpotifyDevice();
            }
            else
            {
                party.CurrentTrack = new Track()
                {
                    Uri = state.Item.Uri,
                    Title = state.Item.Title,
                    Artist = state.Item.Artist,
                    DurationMillis = state.Item.DurationMillis,
                    ExpectedFinishTime = DateTime.UtcNow.AddMilliseconds(state.Item.DurationMillis - state.Item.ProgressMillis)
                };

                if (party.Playback != Playback.NotActive)
                {
                    party.Playback = state.IsPlaying ? Playback.Playing : Playback.Paused;
                }

                party.Owner.CurrentDevice = new SpotifyDevice()
                {
                    DeviceId = state.Device?.DeviceId,
                    Name = state.Device?.Name
                };
            }

            await _partyRepository.Update(party);

            // Notify clients
            await _realTimeService.SendPlaybackStatusUpdate(party, exemptUsers);
        }

        public async Task PlaybackEnded(Party party)
        {
            party = await _partyRepository.GetWithAllProperties(party);
            party.Playback = Playback.NotActive;
            await _partyRepository.Update(party);
            await _realTimeService.SendPlaybackStatusUpdate(party);
        }

        public async Task PlayQueueItem(Party party, QueueItem queueItem, User[] exemptUsers = null)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            if (queueItem == null)
            {
                throw new ArgumentNullException(nameof(queueItem));
            }
            await _spotifyService.PlayTrack(party.Owner, queueItem.SpotifyUri);

            // Todo check if play request worked

            party = await _partyRepository.GetWithAllProperties(party);
            party.CurrentTrack = new Track()
            {
                Uri = queueItem.SpotifyUri,
                Title = queueItem.Title,
                Artist = queueItem.Artist,
                DurationMillis = (int)queueItem.DurationMillis,
                ExpectedFinishTime = DateTime.UtcNow.AddMilliseconds(queueItem.DurationMillis)
            };
            party.Playback = Playback.Playing;

            await _queueItemRepository.Delete(queueItem);
            await _partyRepository.Update(party);

            await StartTimerForNextQueueItem(party);

            // Notify clients
            await _realTimeService.SendPlaybackStatusUpdate(party, exemptUsers);
            await _realTimeService.SendPartyStatusUpdate(party);
        }

        public async Task StartOrResume(Party party)
        {
            party = await _partyRepository.GetWithAllProperties(party);

            switch (party.Playback)
            {
                case Playback.NotActive:
                {
                    var queueItem = await _queueService.GetNextQueueItem(party);
                    if (queueItem == null)
                    {
                        break;
                    }

                    await PlayQueueItem(party, queueItem);
                    break;
                }
                case Playback.Paused:
                {
                    break;
                }
                default:
                {
                    return;
                }
            }
        }
        
        public async Task StartTimerForNextQueueItem(Party party)
        {
            var nextQueueItem = await _queueService.GetNextQueueItem(party);

            var action = nextQueueItem == null
                ? TimerAction.DeactivatePlayback
                : TimerAction.PlayQueueItem;

            var timerDetails = new TimerSpecification()
            {
                Action = action,
                ScheduledTimeUtc = party.CurrentTrack.ExpectedFinishTime,
                Party = party,
                QueueItem = nextQueueItem
            };
            _timerQueueService.Enqueue(timerDetails);
        }
    }
}