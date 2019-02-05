using System.Linq;
using System.Threading.Tasks;
using api.Enums;
using api.Models;
using api.Services.Interfaces;

namespace api.Services
{
    public class PlaybackService : IPlaybackService
    {
        private readonly ITimerQueueService _timerQueueService;
        private readonly ISpotifyService _spotifyService;
        private readonly IPartyService _partyService;

        public PlaybackService(ITimerQueueService timerQueueService, ISpotifyService spotifyService, IPartyService partyService)
        {
            _timerQueueService = timerQueueService;
            _spotifyService = spotifyService;
            _partyService = partyService;
        }

        public async Task StartOrResume(Party party)
        {
            party = await _partyService.LoadFull(party);

            switch (party.Playback)
            {
                case Playback.NOT_ACTIVE:
                {
                    var queueItem = await _partyService.RemoveNextQueueItem(party);
                    if (queueItem == null)
                    {
                        break;
                    }

                    await _spotifyService.PlayTrack(party.Owner, queueItem.SpotifyUri);
                    await _partyService.UpdatePlaybackState(party, queueItem, true);
                    await StartTimerForNextQueueItem(party);
                    break;
                }
                case Playback.PAUSED:
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
            var nextQueueItem = await _partyService.RemoveNextQueueItem(party);
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