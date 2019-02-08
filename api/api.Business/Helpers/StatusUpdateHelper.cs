using System.Collections.Generic;
using System.Linq;
using Api.Domain.DTOs;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Helpers;

namespace Api.Business.Helpers
{
    public class StatusUpdateHelper : IStatusUpdateHelper
    {
        public void CreatePlaybackStatusUpdate(Party party, out PlaybackStatusUpdate memberUpdate, out PlaybackStatusUpdate adminUpdate)
        {
            adminUpdate = new PlaybackStatusUpdate()
            {
                Uri = party.CurrentTrack.Uri,
                Artist = party.CurrentTrack.Artist,
                DurationMillis = party.CurrentTrack.DurationMillis,
                Title = party.CurrentTrack.Title,
                DeviceId = party.Owner.CurrentDevice?.DeviceId,
                DeviceName = party.Owner.CurrentDevice?.Name,
                PlaybackState = party.Playback
            };

            memberUpdate = new PlaybackStatusUpdate()
            {
                Uri = party.CurrentTrack.Uri,
                Artist = party.CurrentTrack.Artist,
                DurationMillis = party.CurrentTrack.DurationMillis,
                Title = party.CurrentTrack.Title,
                PlaybackState = party.Playback
            };
        }

        public void CreatePartyStatusUpdate(Party party, out PartyStatus fullMemberUpdate, out PartyStatus pendingMemberUpdate)
        {
            fullMemberUpdate = new PartyStatus
            {
                Id = party.Id,
                Name = party.Name,
                Owner = new UserPartial(party.Owner),
                PendingMembers = party.PendingMembers?.OrderBy(x => x.JoinedPartyDateTime).Select(m => new UserPartial(m)).ToList() ?? new List<UserPartial>(),
                Members = party.Members?.OrderBy(x => x.JoinedPartyDateTime).Select(m => new UserPartial(m)).ToList() ?? new List<UserPartial>(),
                QueueItems = party.QueueItems.OrderBy(x => x.Index).Select(item => new PartyStatusQueueItem(item)).ToList()
            };

            pendingMemberUpdate = new PartyStatus
            {
                Id = party.Id,
                Name = party.Name,
                Owner = new UserPartial(party.Owner),
                PendingMembers = party.PendingMembers?.OrderBy(x => x.JoinedPartyDateTime).Select(m => new UserPartial(m)).ToList() ?? new List<UserPartial>(),
                Members = new List<UserPartial>(),
                QueueItems = new List<PartyStatusQueueItem>()
            };
        }

    }
}