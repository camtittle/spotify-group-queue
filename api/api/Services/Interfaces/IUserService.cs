using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Models;

namespace api.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetFromClaims(ClaimsPrincipal claimsPrincipal);
        User Create(string username);
        Task<User> FindByUsername(string username);
        Task<User> Find(string id);
        Party GetParty(User user);

    }
}
