using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Exceptions;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace api.Services
{
    public class PartyService : IPartyService
    {
        private readonly apiContext _context;
        private readonly IUserService _userService;

        public PartyService(apiContext context,
                            IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<Party> Create(string ownerUserId, string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(name);
            }

            if (_context.Parties.Any(p => p.Name == name))
            {
                throw new ArgumentException("Party name taken");
            }

            var owner = await _userService.Find(ownerUserId);
            if (owner == null)
            {
                throw new ArgumentException("User does not exist");
            }

            var party = new Party
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Members = new List<User>()
            };
            party.Members.Add(owner);

            owner.Owner = true;
            owner.CurrentParty = party;

            _context.Parties.Add(party);
            await _context.SaveChangesAsync();
            return party;
        }

        public async Task<List<Party>> GetAll()
        {
            return await _context.Parties.ToListAsync();
        }

        public async Task<Party> Find(string id)
        {
            return await _context.Parties.Where(p => p.Id == id).Include(p => p.Members).Include(p => p.PendingMembers).FirstOrDefaultAsync();
        }

        public async Task<Party> FindByUser(string userId)
        {
            return await _context.Users.Where(u => u.Id == userId).Select(u => u.CurrentParty).Include(p => p.Members).FirstOrDefaultAsync();
        }

        public async Task Delete(string partyId)
        {
            var party = await Find(partyId);
            // Remove reference from all members
            party.Members.ForEach(u =>
            {
                u.CurrentParty = null;
                u.Owner = false;
                u.Admin = false;
            });
            _context.Parties.Remove(party);
            _context.SaveChanges();
        }

        public async Task Leave(string userId)
        {
            var user = await _userService.Find(userId);
            user.CurrentParty = null;
            user.Admin = false;
            await _context.SaveChangesAsync();
        }

        public async Task RequestToJoin(string partyId, User user)
        {
            // TODO: check not already in party
            //if 
            var party = await Find(partyId);
            user.PendingParty = party ?? throw new ResourceNotFoundException("Party doesn't exist");
            await _context.SaveChangesAsync();

            // TODO: send notification to admin
        }

        public async Task<bool> Exists(string id)
        {
            return await _context.Parties.AnyAsync(e => e.Id == id);
        }
    }
}
