using Api.Infrastructure.DbContexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ADD db context
            services.AddDbContext<SpotifyAppContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SpotifyAppContext")));

            return services;
        }
    }
}
