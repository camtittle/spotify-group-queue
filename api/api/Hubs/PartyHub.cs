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
using Spotify.Interfaces;
using Spotify.Models;


namespace api.Hubs
{
    public class PartyHub : Hub
    {
        private static IHubContext<PartyHub> _hubContext;
        private readonly IUserService _userService;
        private readonly IPartyService _partyService;
        private readonly ISpotifyClient _spotifyClient;
        private readonly ISpotifyService _spotifyService;

        private static readonly string ADMIN_GROUP_SUFFIX = "ADMIN";

        public PartyHub(IHubContext<PartyHub> hubContext, IUserService userService, IPartyService partyService, ISpotifyClient spotifyClient, ISpotifyService spotifyService)
        {
            _hubContext = hubContext;
            _userService = userService;
            _partyService = partyService;
            _spotifyClient = spotifyClient;
            _spotifyService = spotifyService;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await _userService.GetFromClaims(Context.User);
            var party = _userService.GetParty(user);

            // Add user to a Group with members
            await Groups.AddToGroupAsync(Context.ConnectionId, party.Id);

            // If the user is the owner of the party, add to an Admin Group
            var groupName = party.Id + ADMIN_GROUP_SUFFIX;
            if (user.IsOwner)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _userService.GetFromClaims(Context.User);
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
         * Called from client to request the latest playback state
         */
        [Authorize]
        public async Task<PlaybackStatusUpdate> GetCurrentPlaybackState()
        {
            var user = await _userService.GetFromClaims(Context.User);

            var party = _userService.GetParty(user);

            if (party == null)
            {
                throw new PartyHubException("Cannot get playback state - not member of a party");
            }

            if (party.Owner.SpotifyRefreshToken == null)
            {
                throw new PartyHubException("Cannot get playback state - Party not connected to Spotify");
            }

            party = await _partyService.LoadFull(party);

            // Get latest playback state from Spotify
            var status = await _spotifyService.GetPlaybackState(party.Owner);

            await _partyService.UpdatePlaybackState(party, status);

            return _partyService.GetPlaybackStatusUpdate(party, user.IsOwner);
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
            var owner = await _userService.GetFromClaims(Context.User);
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
        public async Task AddTrackToQueue(AddTrackToQueueRequest requestModel)
        {
            if (requestModel == null)
            {
                throw new ArgumentNullException(nameof(requestModel));
            }

            if (string.IsNullOrWhiteSpace(requestModel.SpotifyUri) || string.IsNullOrWhiteSpace(requestModel.Artist) ||
                string.IsNullOrWhiteSpace(requestModel.Title) || requestModel.DurationMillis < 0)
            {
                throw new ArgumentException("Missing request paramaters", nameof(requestModel));
            }

            var user = await _userService.GetFromClaims(Context.User);

            // TODO: do something with the result?
            var queueItem = await _partyService.AddQueueItem(user, requestModel);

            // Notify other users
            var party = _userService.GetParty(user);
            await SendPartyStatusUpdate(party);
        }

        public async Task RemoveTrackFromQueue(string queueItemId)
        {
            if (string.IsNullOrWhiteSpace(queueItemId))
            {
                throw new ArgumentException("Missing request paramaters", nameof(queueItemId));
            }

            var user = await _userService.GetFromClaims(Context.User);

            await _partyService.RemoveQueueItem(user, queueItemId);

            // Notify other users
            var party = _userService.GetParty(user);
            await SendPartyStatusUpdate(party);
        }

        /*
         * Called on client to search for tracks on Spotify based on a query string
         */
        public async Task<TrackSearchResponse> SearchSpotifyTracks(string query)
        {
            var result = await _spotifyClient.Search(query);

            return result;
        }

        /*
         ********************** Utility methods **************************
         */
        private async Task SendPartyStatusUpdate(Party party)
        {
            party = await _partyService.LoadFull(party);
            var fullModel = await _partyService.GetCurrentParty(party);
            var partialModel = await _partyService.GetCurrentParty(party, true);

            await Clients.Users(party.Members.Select(x => x.Id).ToList()).SendAsync("partyStatusUpdate", fullModel);
            await Clients.Users(party.PendingMembers.Select(x => x.Id).ToList()).SendAsync("partyStatusUpdate", partialModel);
            await Clients.User(party.Owner.Id).SendAsync("partyStatusUpdate", fullModel);
        }

        public static async Task SendPlaybackStatusUpdate(Party party, PlaybackStatusUpdate update, PlaybackStatusUpdate partialUpdate, User[] exceptUsers = null)
        {
            if (exceptUsers == null)
            {
                exceptUsers = new User[] {};
            } 

            var members = party.Members.Except(exceptUsers).Select(x => x.Id).ToList();
            await _hubContext.Clients.Users(members).SendAsync("playbackStatusUpdate", partialUpdate);

            if (!exceptUsers.Contains(party.Owner))
            {
                await _hubContext.Clients.User(party.Owner.Id).SendAsync("playbackStatusUpdate", update);
            }
        }
        
    }
}
