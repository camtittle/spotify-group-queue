using System.Linq;
using System.Security.Claims;
using Api.Domain.Interfaces.Helpers;

namespace Api.Business.Helpers
{
    public class JwtHelper : IJwtHelper
    {
        public string GetUserIdFromToken(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
        }

    }
}