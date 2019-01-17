﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers.Models;
using api.Exceptions;
using api.Hubs;
using api.Hubs.Models;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.Models;
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
                Owner = owner,
                QueueItems = new List<QueueItem>()
            };
            _context.Parties.Add(party);
            await _context.SaveChangesAsync();
            return party;
        }

        public async Task<List<Party>> GetAll()
        {
            return await _context.Parties.Include(p => p.Members).Include(p => p.PendingMembers).Include(p => p.Owner)
                .ToListAsync();
        }

        public async Task<Party> Find(string id)
        {
            return await _context.Parties.Where(p => p.Id == id).Include(p => p.Owner).Include(p => p.Members)
                .Include(p => p.PendingMembers).FirstOrDefaultAsync();
        }

        public async Task Delete(Party party)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            party = await LoadFull(party);

            party.Owner = null;
            party.Members?.ForEach(m => m.CurrentParty = null);
            party.PendingMembers?.ForEach(m => m.PendingParty = null);
            _context.SaveChanges();

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

            // Verify not an owner of a party
            if (user.IsOwner || user.IsMember)
            {
                throw new JoiningPartyException("User already in a party - cannot join another party");
            }

            user.PendingParty = party ?? throw new ArgumentNullException(nameof(party));
            user.JoinedPartyDateTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

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

            await _context.SaveChangesAsync();
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

            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(string id)
        {
            return await _context.Parties.AnyAsync(e => e.Id == id);
        }

        public async Task<CurrentParty> GetCurrentParty(Party party, bool partial = false)
        {
            if (party == null)
            {
                return null;
            }

            party = await LoadFull(party);

            return new CurrentParty(party, partial);
        }

        public async Task<Party> LoadFull(Party party)
        {
            return await _context.Parties.Where(p => p.Id == party.Id).Include(p => p.Members)
                .ThenInclude(u => u.CurrentParty).Include(p => p.PendingMembers).ThenInclude(u => u.PendingParty)
                .Include(p => p.Owner).ThenInclude(u => u.OwnedParty)
                .Include(p => p.QueueItems)
                .Include(p => p.CurrentTrack)
                .FirstOrDefaultAsync();
        }

        public async Task<QueueItem> AddQueueItem(User user, AddTrackToQueueRequest request)
        {
            if (!user.IsOwner && !user.IsMember)
            {
                throw new PartyQueueException("Cannot add request to queue - not a member of a party");
            }

            var party = await LoadFull(_userService.GetParty(user));
            if (party == null)
            {
                throw new PartyQueueException("Cannot add request to queue - party not found");
            }

            var newIndex = party.QueueItems.Count > 0 ? party.QueueItems.Max(x => x.Index) + 1 : 1;

            // TODO: limit number of tracks in queue per user
            var queueItem = new QueueItem()
            {
                AddedByUser = user,
                ForParty = party,
                SpotifyUri = request.SpotifyUri,
                Title = request.Title,
                Artist = request.Artist,
                DurationMillis = request.DurationMillis,
                Index = newIndex
            };

            _context.QueueItems.Add(queueItem);

            await _context.SaveChangesAsync();

            return queueItem;
        }

        public async Task RemoveQueueItem(User user, string queueItemId)
        {
            if (!user.IsOwner)
            {
                throw new PartyQueueException("Cannot remove from queue - not owner of party");
            }

            var party = await LoadFull(_userService.GetParty(user));
            if (party == null)
            {
                throw new PartyQueueException("Cannot remove track from queue - party not found");
            }

            var queueItem = await _context.QueueItems.FindAsync(queueItemId);
            if (queueItem == null)
            {
                throw new PartyQueueException("Cannot remove track from queue - queue item with ID not found");
            }

            _context.QueueItems.Remove(queueItem);

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePlaybackState(Party party, PlaybackState state)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            await LoadFull(party);

            party.CurrentTrack = new Models.Track()
            {
                Uri = state.Item.Uri,
                Title = state.Item.Name,
                Artist = state.Item.Artists[0].Name,
                DurationMillis = state.Item.DurationMillis,
                IsPlaying = state.IsPlaying
            };

            party.Owner.CurrentDevice = new SpotifyDevice()
            {
                DeviceId = state.Device?.Id,
                Name = state.Device?.Name
            };

            // Notify clients
            await SendPlaybackStatusUpdate(party);

            await _context.SaveChangesAsync();
        }

        public PlaybackStatusUpdate GetPlaybackStatusUpdate(Party party, bool includeAdminFields = false)
        {
            var update = new PlaybackStatusUpdate()
            {
                Uri = party.CurrentTrack.Uri,
                Artist = party.CurrentTrack.Artist,
                DurationMillis = party.CurrentTrack.DurationMillis,
                Title = party.CurrentTrack.Title,
                IsPlaying = party.CurrentTrack.IsPlaying
            };

            if (includeAdminFields)
            {
                update.DeviceId = party.Owner.CurrentDevice?.DeviceId;
                update.DeviceName = party.Owner.CurrentDevice?.Name;
            }

            return update;
        }

        public async Task SendPlaybackStatusUpdate(Party party)
        {
            await LoadFull(party);

            var partialUpdate = GetPlaybackStatusUpdate(party);

            var fullUpdate = GetPlaybackStatusUpdate(party, true);

            await PartyHub.SendPlaybackStatusUpdate(party, fullUpdate, partialUpdate);
        }
    }
}
