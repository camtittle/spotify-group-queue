using System.Threading.Tasks;
using api.Controllers.Models;
using api.Models;
using Spotify.Models;
using PlaybackState = Spotify.Models.PlaybackState;

namespace api.Services.Interfaces
{
    public interface ISpotifyService
    {
        Task<SpotifyAccessToken> AuthorizeClient(User user, string code);
        Task<SpotifyAccessToken> GetUserAccessToken(User user);
        Task UpdateUserTokens(User user, string accessToken, string refreshToken, int expiresIn);
        Task<bool> TransferPlayback(User user, string deviceId);
        Task<PlaybackState> GetPlaybackState(User user);
        Task PlayTrack(User user, string[] uris, int startAtMillis = 0);
    }
}