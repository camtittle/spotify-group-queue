using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Spotify.Models;

namespace Spotify.Interfaces
{
    public interface ISpotifyClient
    {
        Task<TrackSearchResponse> Search(string query);
        Task<AuthorizationCodeTokenResponse> GetClientToken(string code, bool isRefreshToken = false);
        Task<T> GetAsUser<T>(string endpoint, string accessToken);
        // Task<T> PostAsUser<T>(string endpoint, string accessToken, object body);
        Task<T> PutAsUser<T>(string endpoint, string accessToken, object jsonBody);
    }
}