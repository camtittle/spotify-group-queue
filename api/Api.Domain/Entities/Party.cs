using System;
using System.Collections.Generic;
using Api.Domain.Enums;

namespace Api.Domain.Entities
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

        public Party()
        {
            Id = Guid.NewGuid().ToString();
            Members = new List<User>();
            PendingMembers = new List<User>();
            QueueItems = new List<QueueItem>();
            Playback = Playback.NotActive;
            CurrentTrack = new Track();
        }
    }
}
