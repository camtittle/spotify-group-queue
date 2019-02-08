using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spotify.Exceptions;

namespace Api.Business.Services
{
    public class TimerBackgroundService : IHostedService
    {
        private IPartyRepository _partyRepository;
        private IPlaybackService _playbackService;
        private readonly ITimerQueueService _timerQueueService;

        private readonly Dictionary<string, Timer> _timers;

        private CancellationTokenSource _tokenSource;

        public IServiceProvider Services { get; }

        public TimerBackgroundService(IServiceProvider services, ITimerQueueService timerQueueService)
        {
            Services = services;
            _timerQueueService = timerQueueService;
            _timers = new Dictionary<string, Timer>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            using (var scope = Services.CreateScope())
            {
                _playbackService = scope.ServiceProvider.GetRequiredService<IPlaybackService>();
                _partyRepository = scope.ServiceProvider.GetRequiredService<IPartyRepository>();

                Listen();
            }

        }

        private void Listen()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                try
                {
                    // Blocks if queue is empty
                    var timerDetails = _timerQueueService.Dequeue();

                    // Indicates a request to remove any timer for the user
                    if (timerDetails.Instruction == TimerInstruction.CancelExistingTimer)
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

        private void AddOrReplaceTimer(TimerSpecification timerSpecification)
        {
            if (timerSpecification.Party == null)
            {
                throw new ArgumentException("Could not set timer: Party was null in timer details");
            }

            var key = timerSpecification.Party.Id;

            // Cancel existing timer
            if (_timers.ContainsKey(key))
            {
                _timers[key]?.Dispose();
            }

            if (timerSpecification.Instruction == TimerInstruction.PlayQueueItem)
            {
                AddTimerForQueueItem(key, timerSpecification);
            } else if (timerSpecification.Instruction == TimerInstruction.DeactivatePlayback)
            {
                AddTimerToDeactivatePlayback(key, timerSpecification);
            }
        }

        private void AddTimerForQueueItem(string key, TimerSpecification timerSpecification)
        {
            Console.WriteLine($"Adding timer to play queue item: {timerSpecification.QueueItem.Title} with delay {timerSpecification.DelayMillis}");
            _timers[key] = new Timer(async state =>
            {
                Console.WriteLine($"Executing timer to play queue item: {timerSpecification.QueueItem.Title} with delay {timerSpecification.DelayMillis}");
                await _playbackService.PlayQueueItem(timerSpecification.Party, timerSpecification.QueueItem, true);

            }, null, timerSpecification.DelayMillis, Timeout.Infinite);
        }

        private void AddTimerToDeactivatePlayback(string key, TimerSpecification timerSpecification)
        {
            Console.WriteLine($"Adding timer to deactivate playback with delay {timerSpecification.DelayMillis}");
            _timers[key] = new Timer(async state =>
            {
                Console.WriteLine($"Executing timer to deactivate playback with delay {timerSpecification.DelayMillis}");
                await _playbackService.PlaybackEnded(timerSpecification.Party);

            }, null, timerSpecification.DelayMillis, Timeout.Infinite);
        }

        private void RemoveTimer(TimerSpecification timerSpecification)
        {
            if (timerSpecification.Party == null)
            {
                throw new ArgumentException("Could not set timer: Party was null in timer details");
            }

            var key = timerSpecification.Party.Id;

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