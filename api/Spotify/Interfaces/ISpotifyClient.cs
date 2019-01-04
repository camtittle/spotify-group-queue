using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Models;

namespace Spotify.Interfaces
{
    public interface ISpotifyClient
    {
        Task<SpotifyTrackSearchResponse> Search(string query);
    }
}