using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Models
{
    public class SpotifySettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TokenUri { get; set; }
        public string RedirectUri { get; set; }
    }
}
