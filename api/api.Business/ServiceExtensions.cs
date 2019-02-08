using Api.Business.Helpers;
using Api.Business.Providers;
using Api.Business.Services;
using Api.Domain.Interfaces.Helpers;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;
using Api.Domain.Settings;
using Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spotify;
using Spotify.Interfaces;

namespace Api.Business
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<IConfiguration>(configuration);

            // Add settings
            services.Configure<SpotifySettings>(configuration.GetSection("SpotifySettings"));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPartyRepository, PartyRepository>();
            services.AddScoped<IQueueItemRepository, QueueItemRepository>();

            // Add services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IQueueService, QueueService>();
            services.AddScoped<IPartyService, PartyService>();
            services.AddScoped<ISpotifyService, SpotifyService>();
            services.AddScoped<IPlaybackService, PlaybackService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IRealTimeService, RealTimeService>();

            services.AddSingleton<ITimerQueueService, TimerQueueService>();

            // Add helpers
            services.AddScoped<IJwtHelper, JwtHelper>();
            services.AddScoped<IStatusUpdateHelper, StatusUpdateHelper>();

            // Backrgound services
            services.AddHostedService<TimerBackgroundService>();

            // SignalR user ID provider
            services.AddSingleton<IUserIdProvider, MyUserIdProvider>();

            // Spotify
            services.AddScoped<ISpotifyClient, SpotifyClient>();
            services.AddSingleton<ISpotifyTokenManager, SpotifyTokenManager>();

            return services;
        }

    }
}