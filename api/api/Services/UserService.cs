using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using api.Exceptions;
using api.Models;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class UserService : IUserService
    {
        private readonly apiContext _context;

        public UserService(apiContext context)
        {
            _context = context;
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

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public async Task<User> Find(string id)
        {
            return await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> FindByUsername(string username)
        {
            return await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
        }
    }
}
