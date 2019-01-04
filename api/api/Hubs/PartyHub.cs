using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Exceptions;
using api.Hubs.Models;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace api.Hubs
{
    public class PartyHub : Hub
    {
        private static IHubContext<PartyHub> _hubContext;
        private IUserService _userService;
        private IPartyService _partyService;

        private static readonly string ADMIN_GROUP_SUFFIX = "ADMIN";

        public PartyHub(IHubContext<PartyHub> hubContext, IUserService userService, IPartyService partyService)
        {
            _hubContext = hubContext;
            _userService = userService;
            _partyService = partyService;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await GetCurrentUser();
            var party = _userService.GetParty(user);

            // Add user to a Group with the party ID as its name
            await Groups.AddToGroupAsync(Context.ConnectionId, party.Id);

            // If the user is the owner of the party, add to a group with name {partyID}ADMIN
            var groupName = party.Id + ADMIN_GROUP_SUFFIX;
            if (user.IsOwner)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }

            // TODO: notify clients of new member
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await GetCurrentUser();
            var party = _userService.GetParty(user);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, party.Id);
            if (user.IsOwner)
            {
                var groupName = party.Id + ADMIN_GROUP_SUFFIX;
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
        }

        public static async Task NotifyAdminNewPendingMember(User user, Party party)
        {
            var userModel = new OtherUser(user);
            var adminGroupName = party.Id + ADMIN_GROUP_SUFFIX;
            await _hubContext.Clients.Group(adminGroupName).SendAsync("onPendingMemberRequest", userModel);
        }

        /*
         * Called from client to accept/decline pending membership request
         */
        // TODO add admin policy?
        [Authorize]
        public async Task AcceptPendingMember(string pendingUserId, bool accept)
        {
            if (string.IsNullOrWhiteSpace(pendingUserId))
            {
                throw new ArgumentNullException(nameof(pendingUserId));
            }

            var pendingUser = await _userService.Find(pendingUserId);
            if (pendingUser == null)
            {
                throw new ArgumentException("User does not exist", nameof(pendingUserId));
            }

            var party = pendingUser.PendingParty;
            if (party == null)
            {
                throw new ArgumentException("User does not have a pending party", nameof(pendingUserId));
            }

            // Verify that the authorized user is the owner of the party, and user is a pending member of the party
            var owner = await GetCurrentUser();
            if (!owner.IsOwner || party.Owner.Id != owner.Id)
            {
                // TODO throw an exception?
                return;
            }

            if (accept)
            {
                await _partyService.AddPendingMember(party, pendingUser);
            }
            else
            {
                await _partyService.RemovePendingMember(party, pendingUser);
            }

            // Notify pending member
            await Clients.User(pendingUserId).SendAsync("pendingMembershipResponse", accept);

            // Notify all users of status update
            await SendPartyStatusUpdate(party);
        }

        /*
         * Called on client to add a song to the queue
         */
        [Authorize]
        public async Task AddTrackToQueue(AddQueueTrack trackModel)
        {
            if (trackModel == null)
            {
                throw new ArgumentNullException(nameof(trackModel));
            }

            if (string.IsNullOrWhiteSpace(trackModel.SpotifyUri) || string.IsNullOrWhiteSpace(trackModel.Artist) ||
                string.IsNullOrWhiteSpace(trackModel.Title) || trackModel.DurationMillis < 0)
            {
                throw new ArgumentException("Missing track paramaters", nameof(trackModel));
            }

            // Determine party of user
            var user = await GetCurrentUser();

            // TODO: do something with the result?
            var queueItem = await _partyService.AddQueueItem(user, trackModel);

            // Notify other users
            var party = _userService.GetParty(user);
            await SendPartyStatusUpdate(party);
        }

        /*
         ********************** Utility methods **************************
         */
        private async Task SendPartyStatusUpdate(Party party)
        {
            var model = await _partyService.GetCurrentParty(party);

            var groupName = party.Id;
            await Clients.Group(groupName).SendAsync("partyStatusUpdate", model);
        }

        public async Task<User> GetCurrentUser()
        {
            var userId = Context.User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
            return await _userService.Find(userId);
        }

        

    }
}
