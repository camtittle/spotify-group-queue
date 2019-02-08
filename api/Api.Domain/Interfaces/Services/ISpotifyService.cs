using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Services
{
    public interface ISpotifyService
    {
        Task<SpotifyAccessToken> AuthorizeClient(User user, string code);
        Task<SpotifyAccessToken> GetUserAccessToken(User user);
        Task UpdateUserTokens(User user, string accessToken, string refreshToken, int expiresIn);
        Task<bool> TransferPlayback(User user, string deviceId);
        Task<SpotifyPlaybackState> GetPlaybackState(User user);
        Task PlayTrack(User user, string uri, int startAtMillis = 0);
        Task PlayTrack(User user, string[] uris, int startAtMillis = 0);
        Task<List<Track>> Search(string query);
    }
}