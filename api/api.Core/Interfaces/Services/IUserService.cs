using System.Security.Claims;
using System.Threading.Tasks;
using api.Domain.Entities;

namespace api.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> GetFromClaims(ClaimsPrincipal claimsPrincipal);
        User Create(string username);
        Task<User> FindByUsername(string username);
        Task<User> Find(string id);
        Task Update(User user);
        Party GetParty(User user);
    }
}
