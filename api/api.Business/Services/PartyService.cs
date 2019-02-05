using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Business.Exceptions;
using Api.Domain.DTOs;
using Api.Domain.Entities;
using Api.Domain.Enums;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;

namespace Api.Business.Services
{
    public class PartyService : IPartyService
    {
        private readonly IPartyRepository _partyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMembershipService _membershipService;
        private readonly ISpotifyService _spotifyService;

        public PartyService(IPartyRepository partyRepository, IUserRepository userRepository, IMembershipService membershipService, ISpotifyService spotifyService)
        {
            _partyRepository = partyRepository;
            _userRepository = userRepository;
            _membershipService = membershipService;
            _spotifyService = spotifyService;
        }

        public async Task<Party> Create(User owner, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(name);
            }

            if (_partyRepository.GetByName(name) != null)
            {
                throw new ArgumentException("Party name taken");
            }

            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            await _membershipService.Leave(owner);
            var result = await _partyRepository.Add(new Party { Name = name, Owner = owner });

            return result;
        }

        public async Task Delete(Party party)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            party = await _partyRepository.GetWithAllProperties(party);

            party.Owner = null;
            party.Members?.ForEach(m => m.CurrentParty = null);
            party.PendingMembers?.ForEach(m => m.PendingParty = null);

            await _partyRepository.Update(party);
            await _partyRepository.Delete(party);
        }

        // ToDo: rename to "SetPlaybackDevice" or similar
        public async Task<SpotifyDevice> UpdateDevice(Party party, string deviceId, string deviceName)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            party = await LoadFull(party);

            var owner = party.Owner;

            if (deviceId == owner.CurrentDevice.DeviceId)
            {
                return owner.CurrentDevice;
            }

            if (deviceId == null)
            {
                return await SavePlaybackDevice(party, deviceId, deviceName);
            }

            var success = await _spotifyService.TransferPlayback(owner, deviceId);

            if (!success)
            {
                return owner.CurrentDevice;
            }

            // Poll Spotify a few times to see if playback device actually changed
            var retryPolicy = Policy
                .HandleResult<PlaybackState>(state => state?.Device?.Id != deviceId)
                .WaitAndRetryAsync(
                    4,
                    count => TimeSpan.FromSeconds(1));

            var finalState = await retryPolicy.ExecuteAsync(async () => await _spotifyService.GetPlaybackState(owner));

            return await SavePlaybackDevice(party, finalState.Device.Id, finalState.Device.Name);
        }

        private async Task<SpotifyDevice> SavePlaybackDevice(Party party, string id, string name)
        {
            party.Owner.CurrentDevice = new SpotifyDevice()
            {
                DeviceId = id,
                Name = name
            };

            await _userRepository.Update(party.Owner);

            return party.Owner.CurrentDevice;
        }

        public async Task UpdatePlaybackState(Party party, PlaybackState state, User[] dontNotifyUsers = null)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            party = await LoadFull(party);

            if (state == null)
            {
                party.CurrentTrack = new Models.Track();

                party.Playback = Playback.NOT_ACTIVE;

                party.Owner.CurrentDevice = new SpotifyDevice();
            }
            else
            {
                party.CurrentTrack = new Models.Track()
                {
                    Uri = state.Item.Uri,
                    Title = state.Item.Name,
                    Artist = state.Item.Artists[0].Name,
                    DurationMillis = state.Item.DurationMillis
                };

                if (party.Playback != Playback.NOT_ACTIVE)
                {
                    party.Playback = state.IsPlaying ? Playback.PLAYING : Playback.PAUSED;
                }

                party.Owner.CurrentDevice = new SpotifyDevice()
                {
                    DeviceId = state.Device?.Id,
                    Name = state.Device?.Name
                };
            }
            
            // Notify clients
            await SendPlaybackStatusUpdate(party, dontNotifyUsers);

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePlaybackState(Party party, QueueItem queueItem, bool isPlaying, User[] dontNotifyUsers = null)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            if (queueItem == null)
            {
                throw new ArgumentNullException(nameof(queueItem));
            }

            party = await LoadFull(party);

            party.CurrentTrack = new Models.Track()
            {
                Uri = queueItem.SpotifyUri,
                Title = queueItem.Title,
                Artist = queueItem.Artist,
                DurationMillis = (int) queueItem.DurationMillis
            };

            party.Playback = isPlaying ? Playback.PLAYING : Playback.PAUSED;

            // Notify clients
            await SendPlaybackStatusUpdate(party, dontNotifyUsers);

            await _context.SaveChangesAsync();
        }

        public PlaybackStatusUpdate GetPlaybackStatusUpdate(Party party, bool includeAdminFields = false)
        {
            var update = new PlaybackStatusUpdate()
            {
                Uri = party.CurrentTrack.Uri,
                Artist = party.CurrentTrack.Artist,
                DurationMillis = party.CurrentTrack.DurationMillis,
                Title = party.CurrentTrack.Title
            };

            if (includeAdminFields)
            {
                update.DeviceId = party.Owner.CurrentDevice?.DeviceId;
                update.DeviceName = party.Owner.CurrentDevice?.Name;
                update.PlaybackState = party.Playback;
            }

            return update;
        }

        public async Task SendPlaybackStatusUpdate(Party party, User[] exceptUsers = null)
        {
            party = await LoadFull(party);

            var partialUpdate = GetPlaybackStatusUpdate(party);

            var fullUpdate = GetPlaybackStatusUpdate(party, true);

            await PartyHub.SendPlaybackStatusUpdate(party, fullUpdate, partialUpdate, exceptUsers);
        }

        public async Task<QueueItem> RemoveNextQueueItem(Party party)
        {
            if (party.QueueItems == null)
            {
                party = await LoadFull(party);
            }

            if (party.QueueItems.Count > 0)
            {
                var queueItem = party.QueueItems.OrderBy(x => x.Index).First();

                party.QueueItems.Remove(queueItem);

                await _context.SaveChangesAsync();

                return queueItem;
            }

            return null;
        }
    }
}
