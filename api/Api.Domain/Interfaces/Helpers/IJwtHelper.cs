using System.Security.Claims;

namespace Api.Domain.Interfaces.Helpers
{
    public interface IJwtHelper
    {
        string GetUserIdFromToken(ClaimsPrincipal claimsPrincipal);
    }
}
