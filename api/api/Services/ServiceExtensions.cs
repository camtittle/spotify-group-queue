using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Providers;
using api.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace api.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Add all services here
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPartyService, PartyService>();

            // SignalR user ID provider
            services.AddSingleton<IUserIdProvider, MyUserIdProvider>();

            return services;
        }
    }
}
