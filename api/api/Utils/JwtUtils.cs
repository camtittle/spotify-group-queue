using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Utils
{
    public static class JwtUtils
    {
        public static string GetUserIdFromToken(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
        }
    }
}
