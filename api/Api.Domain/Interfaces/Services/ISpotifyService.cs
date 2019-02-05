using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Entities;
using PlaybackState = Spotify.Models.PlaybackState;

namespace Api.Domain.Interfaces.Services
{
    public interface ISpotifyService
    {
        Task<SpotifyAccessToken> AuthorizeClient(User user, string code);
        Task<SpotifyAccessToken> GetUserAccessToken(User user);
        Task UpdateUserTokens(User user, string accessToken, string refreshToken, int expiresIn);
        Task<bool> TransferPlayback(User user, string deviceId);
        Task<PlaybackState> GetPlaybackState(User user);
        Task PlayTrack(User user, string uri, int startAtMillis = 0);
        Task PlayTrack(User user, string[] uris, int startAtMillis = 0);
    }
}