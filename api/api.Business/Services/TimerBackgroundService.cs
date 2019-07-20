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

            Listen();
        }

        private void Listen()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                try
                {
                    // Blocks if queue is empty
                    var timerAction = _timerQueueService.Dequeue();
                    using (var scope = Services.CreateScope())
                    {
                        _playbackService = scope.ServiceProvider.GetRequiredService<IPlaybackService>();
                        HandleTimerAction(timerAction);
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

        private void HandleTimerAction(TimerSpecification timerSpecification)
        {
            if (timerSpecification.Party == null)
            {
                throw new ArgumentException("Could not set timer: Party was null in timer details");
            }

            var key = timerSpecification.Party.Id;
            DisposeTimer(key);

            switch (timerSpecification.Action)
            {
                case TimerAction.PlayQueueItem:
                    SetTimer(key, timerSpecification, async state =>
                    {
                        using (var scope = Services.CreateScope())
                        {
                            _playbackService = scope.ServiceProvider.GetRequiredService<IPlaybackService>();
                            await _playbackService.PlayQueueItem(timerSpecification.Party, timerSpecification.QueueItem);
                        }
                    });
                    break;
                case TimerAction.DeactivatePlayback:
                    SetTimer(key, timerSpecification, async state =>
                    {
                        await _playbackService.PlaybackEnded(timerSpecification.Party);
                    });
                    break;
                case TimerAction.CancelExistingTimer:
                    // No action required
                    break;
            }
        }

        private void SetTimer(string key, TimerSpecification timerSpecification, TimerCallback callback)
        {
            var span = timerSpecification.ScheduledTimeUtc - DateTime.UtcNow;

            var calcDelayMillis = (int)Math.Abs(span.TotalMilliseconds);

            Console.WriteLine($"Adding timer with action: {timerSpecification.Action.ToString()} with delay: {calcDelayMillis}ms");
            _timers[key] = new Timer(callback, null, calcDelayMillis, Timeout.Infinite);
        }

        private void DisposeTimer(string key)
        {
            Console.WriteLine($"Disposing timer with key: {key}");
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