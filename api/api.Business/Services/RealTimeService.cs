using System.Linq;
using System.Threading.Tasks;
using Api.Business.Hubs;
using Api.Domain.DTOs;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Helpers;
using Api.Domain.Interfaces.Hubs;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace Api.Business.Services
{
    /*
     * Sends real time updates to SignalR clients
     */
    public class RealTimeService : IRealTimeService
    {
        private readonly IHubContext<PartyHub, IPartyHubClientMethods> _hubContext;
        private readonly IPartyRepository _partyRepository;
        private readonly IStatusUpdateHelper _partyStatusUpdateHelper;

        public RealTimeService(IHubContext<PartyHub, IPartyHubClientMethods> hubContext, IPartyRepository partyRepository, IStatusUpdateHelper partyStatusUpdateHelper)
        {
            _hubContext = hubContext;
            _partyRepository = partyRepository;
            _partyStatusUpdateHelper = partyStatusUpdateHelper;
        }

        public async Task NotifyOwnerOfPendingMember(User user, Party party)
        {
            var userModel = new UserPartial(user);
            party = await _partyRepository.GetWithAllProperties(party);

            await _hubContext.Clients.User(party.Owner.Id).onPendingMemberRequest(userModel);
        }

        public async Task SendPlaybackStatusUpdate(Party party, User[] exemptUsers = null)
        {
            party = await _partyRepository.GetWithAllProperties(party);

            _partyStatusUpdateHelper.CreatePlaybackStatusUpdate(party, out var memberUpdate, out var adminUpdate);

            if (exemptUsers == null)
            {
                exemptUsers = new User[] { };
            }

            var members = party.Members.Except(exemptUsers).Select(x => x.Id).ToList();
            members.ForEach(async member =>
            {
                await _hubContext.Clients.User(member).playbackStatusUpdate(memberUpdate);
            });

            if (!exemptUsers.Contains(party.Owner))
            {
                await _hubContext.Clients.User(party.Owner.Id).playbackStatusUpdate(adminUpdate);
            }
        }

        public async Task SendPartyStatusUpdate(Party party)
        {
            party = await _partyRepository.GetWithAllProperties(party);

            _partyStatusUpdateHelper.CreatePartyStatusUpdate(party, out var fullMemberUpdate, out var pendingMemberUpdate);

            await _hubContext.Clients.Users(party.Members.Select(x => x.Id).ToList()).partyStatusUpdate(fullMemberUpdate);
            await _hubContext.Clients.Users(party.PendingMembers.Select(x => x.Id).ToList()).partyStatusUpdate(pendingMemberUpdate);
            await _hubContext.Clients.User(party.Owner.Id).partyStatusUpdate(fullMemberUpdate);
        }
    }
}