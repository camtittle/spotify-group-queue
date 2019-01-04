using api.Providers;
using api.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spotify;
using Spotify.Interfaces;

namespace api.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add all services here
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPartyService, PartyService>();
            services.AddScoped<ISpotifyClient, SpotifyClient>();

            // SignalR user ID provider
            services.AddSingleton<IUserIdProvider, MyUserIdProvider>();

            // Spotify
            services.AddSingleton<ISpotifyTokenManager>(serviceProvider =>
            {
                var clientId = configuration["SpotifySettings:ClientId"];
                var clientSecret = configuration["SpotifyClientSecret"];
                return new SpotifyTokenManager(clientId, clientSecret);
            });

            return services;
        }
    }
}
