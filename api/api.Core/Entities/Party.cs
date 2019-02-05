using System.Collections.Generic;
using api.Domain.Enums;

namespace api.Domain.Entities
{
    public class Party
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public User Owner { get; set; }

        public List<User> Members { get; set; }

        public List<User> PendingMembers { get; set; }

        public List<QueueItem> QueueItems { get; set; }

        public Track CurrentTrack { get; set; }

        public Playback Playback { get; set; }
    }
}
