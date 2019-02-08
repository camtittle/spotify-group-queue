using System.Collections.Generic;
using System.Linq;
using Api.Domain.Entities;

namespace Api.Domain.DTOs
{
    public class PartyStatus
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public UserPartial Owner { get; set; }

        public List<UserPartial> Members { get; set; }

        public List<UserPartial> PendingMembers { get; set; }

        public List<PartyStatusQueueItem> QueueItems { get; set; }
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
