using System;
using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Helpers;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;
using Polly;

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
            var party = new Party { Name = name, Owner = owner };
            await _partyRepository.Add(party);

            return party;
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

            party = await _partyRepository.GetWithAllProperties(party);

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

            // ToDo: Move this to spotify service
            // Poll Spotify a few times to see if playback device actually changed
            var retryPolicy = Policy
                .HandleResult<SpotifyPlaybackState>(state => state?.Device?.DeviceId != deviceId)
                .WaitAndRetryAsync(
                    4,
                    count => TimeSpan.FromSeconds(1));

            var finalState = await retryPolicy.ExecuteAsync(async () => await _spotifyService.GetPlaybackState(owner));

            return await SavePlaybackDevice(party, finalState?.Device?.DeviceId, finalState?.Device?.Name);
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
    }
}
