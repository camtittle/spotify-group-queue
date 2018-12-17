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

        public async Task<Party> Create(User owner, string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(name);
            }

            if (_context.Parties.Any(p => p.Name == name))
            {
                throw new ArgumentException("Party name taken");
            }
            
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            // Leave any exisiting parties
            await Leave(owner);

            var party = new Party
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Members = new List<User>(),
                PendingMembers = new List<User>(),
                Owner = owner
            };
            _context.Parties.Add(party);
            await _context.SaveChangesAsync();
            return party;
        }

        public async Task<List<Party>> GetAll()
        {
            return await _context.Parties.Include(p => p.Members).Include(p => p.PendingMembers).Include(p => p.Owner).ToListAsync();
        }

        public async Task<Party> Find(string id)
        {
            return await _context.Parties.Where(p => p.Id == id).Include(p => p.Owner).Include(p => p.Members).Include(p => p.PendingMembers).FirstOrDefaultAsync();
        }

        public async Task Delete(Party party)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            // Remove reference from all members
            party.Members?.ForEach(u => u.CurrentParty = null);
            party.PendingMembers?.ForEach(u => u.PendingParty = null);
            party.Owner.OwnedParty = null;

            _context.Parties.Remove(party);
            _context.SaveChanges();
        }

        public async Task Leave(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.CurrentParty = null;
            user.PendingParty = null;
            user.OwnedParty = null;
            await _context.SaveChangesAsync();
        }

        public async Task RequestToJoin(Party party, User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // TODO: check not already in party
            user.PendingParty = party ?? throw new ArgumentNullException(nameof(party));
            await _context.SaveChangesAsync();

            // TODO: send notification to admin
        }

        public async Task<bool> Exists(string id)
        {
            return await _context.Parties.AnyAsync(e => e.Id == id);
        }
    }
}
