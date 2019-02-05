using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Business.Services
{
    public class UserService : IUserService
    {
        private readonly apiContext _context;

        public UserService(apiContext context)
        {
            _context = context;
        }

        public async Task<User> GetFromClaims(ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
            var user = await Find(userId);
            return user;
        }

        public User Create(string username)
        {
            // validation
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));
            
            if (_context.Users.Any(x => x.Username == username))
                throw new ArgumentException($"Username {username} is already taken");

            var user = new User
            {
                Username = username,
                Id = Guid.NewGuid().ToString()
            };

            return user;
        }

        public Party GetParty(User user)
        {
            if (user.IsMember)
            {
                return user.CurrentParty;
            }

            if (user.IsOwner)
            {
                return user.OwnedParty;
            }

            if (user.IsPendingMember)
            {
                return user.PendingParty;
            }

            return null;
        }

        
    }
}
