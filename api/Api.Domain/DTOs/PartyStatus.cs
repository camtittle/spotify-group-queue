using System.Collections.Generic;
using System.Linq;
using Api.Domain.Entities;

namespace Api.Domain.DTOs
{
    public class PartyStatus
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public OtherUser Owner { get; set; }

        public List<OtherUser> Members { get; set; }

        public List<OtherUser> PendingMembers { get; set; }

        public List<PartyStatusQueueItem> QueueItems { get; set; }

        public PartyStatus(Party party, bool pending = false)
        {
            Id = party.Id;
            Name = party.Name;
            Owner = new OtherUser(party.Owner);
            PendingMembers = party.PendingMembers?.OrderBy(x => x.JoinedPartyDateTime).Select(m => new OtherUser(m)).ToList() ?? new List<OtherUser>();

            if (!pending)
            {
                Members = party.Members?.OrderBy(x => x.JoinedPartyDateTime).Select(m => new OtherUser(m)).ToList() ?? new List<OtherUser>();
                QueueItems = party.QueueItems.OrderBy(x => x.Index).Select(item => new PartyStatusQueueItem(item)).ToList();
            }
            else
            {
                Members = new List<OtherUser>();
                QueueItems = new List<PartyStatusQueueItem>();
            }

        }
    }

    public class PartyStatusQueueItem
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public string SpotifyUri { get; set; }

        public long DurationMillis { get; set; }

        public PartyStatusQueueItem(QueueItem queueItem)
        {
            Id = queueItem.Id;
            Username = queueItem.AddedByUser?.Username;
            Title = queueItem.Title;
            Artist = queueItem.Artist;
            SpotifyUri = queueItem.SpotifyUri;
            DurationMillis = queueItem.DurationMillis;
        }
    }
}
