using System.Collections.Generic;
using System.Linq;
using Spotify.Models;

namespace Spotify
{
    public static class Utils
    {
        public static string ArtistsToCommaSeparatedString(IEnumerable<ArtistSimplified> artists)
        {
            return string.Join(", ", artists.Select(x => x.Name));
        }
    }
}