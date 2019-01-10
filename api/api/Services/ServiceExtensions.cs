using api.Providers;
using api.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Interfaces;
using Spotify;
using Spotify.Models;

namespace api.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<IConfiguration>(configuration);

            // Add settings
            services.Configure<SpotifySettings>(configuration.GetSection("SpotifySettings"));

            // Add all services here
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPartyService, PartyService>();
            services.AddScoped<ISpotifyClient, SpotifyClient>();
            services.AddScoped<ISpotifyService, SpotifyService>();

            // SignalR user ID provider
            services.AddSingleton<IUserIdProvider, MyUserIdProvider>();

            // Spotify
            services.AddSingleton<ISpotifyTokenManager, SpotifyTokenManager>();

            return services;
        }
    }
}
