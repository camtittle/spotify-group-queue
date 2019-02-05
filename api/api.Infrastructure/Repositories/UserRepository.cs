using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Repositories;
using Api.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SpotifyAppContext _context;

        public UserRepository(SpotifyAppContext context)
        {
            _context = context;
        }

        public async Task Add(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException();
            }

           _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException();
            }

            return await _context.Users.Where(u => u.Id == id)
                .Include(u => u.CurrentParty)
                .Include(u => u.OwnedParty)
                .Include(u => u.PendingParty)
                .Include(u => u.CurrentDevice)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException();
            }

            return await _context.Users.Where(u => u.Username == username)
                .Include(u => u.CurrentParty)
                .Include(u => u.OwnedParty)
                .Include(u => u.PendingParty)
                .Include(u => u.CurrentDevice)
                .FirstOrDefaultAsync();
        }

        public async Task Update(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException();
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}