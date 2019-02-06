using System;
using System.Threading.Tasks;
using Api.Business.Exceptions;
using Api.Business.Hubs;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;

namespace Api.Business.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IUserRepository _userRepository;

        public MembershipService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Leave(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException();
            }

            user.CurrentParty = null;
            user.PendingParty = null;
            user.OwnedParty = null;
            await _userRepository.Update(user);
        }

        public async Task RequestToJoin(Party party, User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Verify not an owner of a party
            if (user.IsOwner || user.IsMember)
            {
                throw new JoiningPartyException("User already in a party - cannot join another party");
            }

            user.PendingParty = party ?? throw new ArgumentNullException(nameof(party));
            user.JoinedPartyDateTime = DateTime.UtcNow;
            await _userRepository.Update(user);

            // Notify admin of pending request
            await PartyHub.NotifyAdminNewPendingMember(user, party);
        }

        public async Task AddPendingMember(Party party, User user)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Verify they are not already in the party
            if (user.CurrentParty?.Id == party.Id)
            {
                return;
            }

            user.CurrentParty = party;
            user.PendingParty = null;
            user.OwnedParty = null;
            user.JoinedPartyDateTime = DateTime.UtcNow;

            await _userRepository.Update(user);
        }

        public async Task RemovePendingMember(Party party, User user)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PendingParty = null;

            await _userRepository.Update(user);
        }
    }
}