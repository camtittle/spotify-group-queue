using System.Security.Claims;

namespace Api.Business.Providers
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