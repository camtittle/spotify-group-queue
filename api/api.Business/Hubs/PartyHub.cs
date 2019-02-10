using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Business.Exceptions;
using Api.Domain.DTOs;
using Api.Domain.Interfaces.Helpers;
using Api.Domain.Interfaces.Hubs;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Track = Api.Domain.Entities.Track;

namespace Api.Business.Hubs
{
    public class PartyHub : Hub<IPartyHubClientMethods>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPartyRepository _partyRepository;
        private readonly IQueueItemRepository _queueItemRepository;

        private readonly ISpotifyService _spotifyService;
        private readonly IPlaybackService _playbackService;
        private readonly IMembershipService _membershipService;
        private readonly IQueueService _queueService;

        private readonly IJwtHelper _jwtHelper;
        private readonly IStatusUpdateHelper _statusUpdateHelper;

        private const string AdminGroupSuffix = "ADMIN";

        public PartyHub(IUserRepository userRepository, IPartyRepository partyRepository, IQueueItemRepository queueItemRepository, ISpotifyService spotifyService, IPlaybackService playbackService, IMembershipService membershipService, IQueueService queueService, IJwtHelper jwtHelper, IStatusUpdateHelper statusUpdateHelper)
        {
            _userRepository = userRepository;
            _partyRepository = partyRepository;
            _queueItemRepository = queueItemRepository;
            _spotifyService = spotifyService;
            _playbackService = playbackService;
            _membershipService = membershipService;
            _queueService = queueService;
            _jwtHelper = jwtHelper;
            _statusUpdateHelper = statusUpdateHelper;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = _jwtHelper.GetUserIdFromToken(Context.User);
            var user = await _userRepository.GetById(userId);
            var party = user.GetActiveParty();

            // Add user to a Group with members
            await Groups.AddToGroupAsync(Context.ConnectionId, party.Id);

            // If the user is the owner of the party, add to an Admin Group
            var groupName = party.Id + AdminGroupSuffix;
            if (user.IsOwner)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _jwtHelper.GetUserIdFromToken(Context.User);
            var user = await _userRepository.GetById(userId);
            var party = user.GetActiveParty();

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, party.Id);
            if (user.IsOwner)
            {
                var groupName = party.Id + AdminGroupSuffix;
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
        }

        /*
         * Called from client to request the latest playback state
         */
        [Authorize]
        public async Task<PlaybackStatusUpdate> GetCurrentPlaybackState()
        {
            var userId = _jwtHelper.GetUserIdFromToken(Context.User);
            var user = await _userRepository.GetById(userId);
            var party = user.GetActiveParty();

            if (party == null)
            {
                throw new PartyHubException("Cannot get playback state - not member of a party");
            }

            if (party.Owner.SpotifyRefreshToken == null)
            {
                throw new PartyHubException("Cannot get playback state - Party not connected to Spotify");
            }

            party = await _partyRepository.GetWithAllProperties(party);

            // Get latest playback state from Spotify
            var status = await _spotifyService.GetPlaybackState(party.Owner);

            await _playbackService.UpdatePlaybackState(party, status, new[] {user});

            _statusUpdateHelper.CreatePlaybackStatusUpdate(party, out var memberUpdate, out var adminUpdate);
            return user.IsOwner ? adminUpdate : memberUpdate;
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

            var pendingUser = await _userRepository.GetById(pendingUserId);
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
            var ownerId = _jwtHelper.GetUserIdFromToken(Context.User);
            var owner = await _userRepository.GetById(ownerId);
            if (!owner.IsOwner || party.Owner.Id != owner.Id)
            {
                // TODO throw an exception?
                return;
            }

            if (accept)
            {
                await _membershipService.AddPendingMember(party, pendingUser);
            }
            else
            {
                await _membershipService.RemovePendingMember(party, pendingUser);
            }

            // Notify pending member
            await Clients.User(pendingUserId).pendingMembershipResponse(accept);
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

            var userId = _jwtHelper.GetUserIdFromToken(Context.User);
            var user = await _userRepository.GetById(userId);

            // TODO: do something with the result?
            await _queueService.AddQueueItem(user, requestModel, true);
        }

        [Authorize]
        public async Task RemoveTrackFromQueue(string queueItemId)
        {
            if (string.IsNullOrWhiteSpace(queueItemId))
            {
                throw new ArgumentException("Missing request paramaters", nameof(queueItemId));
            }

            // ToDo: Gracefully handle when user is not the owner

            var userId = _jwtHelper.GetUserIdFromToken(Context.User);
            var user = await _userRepository.GetById(userId);
            if (!user.IsOwner)
            {
                throw new PartyQueueException("Cannot remove queue item - user does not have permission");
            }

            var queueItem = await _queueItemRepository.Get(queueItemId);
            await _queueItemRepository.Delete(queueItem);
            // Todo send party status update
        }

        /*
         * Called on client to search for tracks on Spotify based on a query string
         */
        [Authorize]
        public async Task<List<Track>> SearchSpotifyTracks(string query)
        {
            var result = await _spotifyService.Search(query);

            return result;
        }

        /*
         * Called on client to activate queue playback
         */
        [Authorize]
        public async Task<bool> ActivateQueuePlayback()
        {
            var userId = _jwtHelper.GetUserIdFromToken(Context.User);
            var user = await _userRepository.GetById(userId);

            if (!user.IsOwner)
            {
                return false;
            }

            var party = user.GetActiveParty();
            await _playbackService.StartOrResume(party);

            return true;
        }
    }
}
