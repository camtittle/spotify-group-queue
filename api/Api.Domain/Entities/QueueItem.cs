using System;

namespace Api.Domain.Entities
{
    public class QueueItem
    {
        public string Id { get; set; }

        public int Index { get; set; }

        public User AddedByUser { get; set; }

        public Party ForParty { get; set; }

        public string SpotifyUri { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public long DurationMillis { get; set; }

        public QueueItem()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}