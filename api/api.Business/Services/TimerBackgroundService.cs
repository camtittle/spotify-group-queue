using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace api.Business.Services
{
    public class TimerBackgroundService : IHostedService
    {

        private readonly TimerQueueService _timerQueueService;
        private readonly ISpotifyService _spotifyService;
        private readonly IPartyService _partyService;
        private readonly Dictionary<string, Timer> _timers;

        private CancellationTokenSource _tokenSource;

        public TimerBackgroundService(TimerQueueService timerQueueService, ISpotifyService spotifyService, IPartyService partyService)
        {
            _timerQueueService = timerQueueService;
            _spotifyService = spotifyService;
            _partyService = partyService;
            _timers = new Dictionary<string, Timer>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            while (!_tokenSource.IsCancellationRequested)
            {
                try
                {
                    // Blocks if queue is empty
                    var timerDetails = _timerQueueService.Dequeue();

                    // Indicates a request to remove any timer for the user
                    if (timerDetails.TrackUri == null)
                    {
                        RemoveTimer(timerDetails);
                    }
                    else
                    {
                        AddOrReplaceTimer(timerDetails);
                    }
                }
                catch (OperationCanceledException)
                {
                    // ToDo: Tidy up timers
                    // ToDo: communicate to clients that server has failed, try again later
                }
                catch (SpotifyAuthenticationException)
                {
                    // ToDo: log exception
                }
            }
        }

        private void AddOrReplaceTimer(TimerDetails timerDetails)
        {
            if (timerDetails.Party == null)
            {
                throw new ArgumentException("Could not set timer: Party was null in timer details");
            }

            var key = timerDetails.Party.Id;

            // Cancel existing timer
            if (_timers.ContainsKey(key))
            {
                _timers[key]?.Dispose();
            }

            _timers[key] = new Timer(async state =>
            {
                var party = await _partyService.LoadFull(timerDetails.Party);

                await _spotifyService.PlayTrack(party.Owner, new[] {timerDetails.TrackUri});

                // TODO: create a new timer for the next song in queue
            }, null, timerDetails.DelayMillis, Timeout.Infinite);
        }

        private void RemoveTimer(TimerDetails timerDetails)
        {
            if (timerDetails.Party == null)
            {
                throw new ArgumentException("Could not set timer: Party was null in timer details");
            }

            var key = timerDetails.Party.Id;

            // Cancel existing timer
            if (_timers.Remove(key, out var timer))
            {
               timer?.Dispose();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();

            // ToDo: wait?
        }

    }
}