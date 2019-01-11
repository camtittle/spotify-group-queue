using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public Party OwnedParty { get; set; }

        public bool IsOwner { get; set; }

        public Party CurrentParty { get; set; }

        public bool IsMember { get; set; }

        public Party PendingParty { get; set; }

        public bool IsPendingMember { get; set; }

        public DateTime JoinedPartyDateTime { get; set; }

        public List<QueueItem> QueueItems { get; set; }

        public string SpotifyAccessToken { get; set; }
        
        public string SpotifyRefreshToken { get; set; }

        public DateTime? SpotifyTokenExpiry { get; set; }

        public string SpotifyDeviceName { get; set; }

        public string SpotifyDeviceId { get; set; }
    }
}
