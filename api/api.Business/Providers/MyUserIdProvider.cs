using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace api.Providers
{
    public class MyUserIdProvider : IUserIdProvider
    {
        // Provides the user ID to use as the ID for signalR hub connections
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.PrimarySid)?.Value;
        }
    }
}