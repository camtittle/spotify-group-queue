using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Exceptions;
using api.Models;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
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
                throw new APIException("Party name cannot be empty");
            }

            if (_context.Parties.Any(p => p.Name == name))
            {
                throw new APIException("Party name taken");
            }

            var owner = await _userService.Find(ownerUserId);

            var party = new Party
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Members = new List<User>()
            };
            party.Members.Add(owner);

            owner.Owner = true;
            owner.CurrentParty = party;

            await _context.Parties.AddAsync(party);
            await _context.SaveChangesAsync();
            return party;
        }

        public async Task<Party> Find(string id)
        {
            return await _context.Parties.FindAsync(id);
        }

        public async Task<Party> FindByUser(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user.CurrentParty;
        }

        public async Task Delete(string partyId)
        {
            var party = await _context.Parties.Where(p => p.Id == partyId).Include(p => p.Members).FirstOrDefaultAsync();
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

        public async Task RequestToJoin(Party party, User user)
        {
            // TODO
        }
    }
}
