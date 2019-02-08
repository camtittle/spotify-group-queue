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

        private readonly IQueueService _queueService;
        private readonly ISpotifyService _spotifyService;
        private readonly ITimerQueueService _timerQueueService;
        private readonly IRealTimeService _realTimeService;

        public PlaybackService(IPartyRepository partyRepository, IQueueService queueService, ISpotifyService spotifyService, ITimerQueueService timerQueueService, IRealTimeService realTimeService)
        {
            _partyRepository = partyRepository;
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
                    DurationMillis = state.Item.DurationMillis
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

        public async Task UpdatePlaybackState(Party party, QueueItem queueItem, bool isPlaying, User[] exemptUsers = null)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            if (queueItem == null)
            {
                throw new ArgumentNullException(nameof(queueItem));
            }

            party = await _partyRepository.GetWithAllProperties(party);
            party.CurrentTrack = new Track()
            {
                Uri = queueItem.SpotifyUri,
                Title = queueItem.Title,
                Artist = queueItem.Artist,
                DurationMillis = (int)queueItem.DurationMillis
            };
            party.Playback = isPlaying ? Playback.Playing : Playback.Paused;

            await _partyRepository.Update(party);

            // Notify clients
            await _realTimeService.SendPlaybackStatusUpdate(party, exemptUsers);
        }

        public async Task StartOrResume(Party party)
        {
            party = await _partyRepository.GetWithAllProperties(party);

            switch (party.Playback)
            {
                case Playback.NotActive:
                {
                    var queueItem = await _queueService.RemoveNextQueueItem(party);
                    if (queueItem == null)
                    {
                        break;
                    }

                    await _spotifyService.PlayTrack(party.Owner, queueItem.SpotifyUri);
                    await UpdatePlaybackState(party, queueItem, true);
                    await StartTimerForNextQueueItem(party);
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
        
        private async Task StartTimerForNextQueueItem(Party party)
        {
            var nextQueueItem = await _queueService.RemoveNextQueueItem(party);
            if (nextQueueItem != null)
            {
                var timerDetails = new TimerDetails()
                {
                    DelayMillis = (int)nextQueueItem.DurationMillis,
                    Party = party,
                    TrackUri = nextQueueItem.SpotifyUri
                };
                _timerQueueService.Enqueue(timerDetails);
            }
        }
    }
}