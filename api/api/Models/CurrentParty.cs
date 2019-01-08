using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace api.Models
{
    public class CurrentParty
    {
        public string Id;

        public string Name;

        public OtherUser Owner;

        public List<OtherUser> Members;

        public List<OtherUser> PendingMembers;

        public List<CurrentPartyQueueItem> QueueItems;

        public CurrentParty(Party party)
        {
            Id = party.Id;
            Name = party.Name;
            Owner = new OtherUser(party.Owner);
            Members = party.Members?.OrderBy(x => x.JoinedPartyDateTime).Select(m => new OtherUser(m)).ToList() ?? new List<OtherUser>();
            PendingMembers = party.PendingMembers?.OrderBy(x => x.JoinedPartyDateTime).Select(m => new OtherUser(m)).ToList() ?? new List<OtherUser>();
            QueueItems = party.QueueItems.OrderBy(x => x.Index).Select(item => new CurrentPartyQueueItem(item)).ToList();

        }
    }

    public class CurrentPartyQueueItem
    {
        public string Id;

        public string Username;

        public string Title;

        public string Artist;

        public string SpotifyUri;

        public long DurationMillis;

        public CurrentPartyQueueItem(QueueItem queueItem)
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
