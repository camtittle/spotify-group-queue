using System.Threading.Tasks;
using api.Controllers.Models;
using api.Models;

namespace api.Services.Interfaces
{
    public interface ISpotifyService
    {
        Task<SpotifyAuthorizationResponse> AuthorizeClient(User user, string code);
        Task<SpotifyAuthorizationResponse> RefreshClientToken(User user);
        Task UpdateUserTokens(User user, string accessToken, string refreshToken, int expiresIn);
        Task UpdateDevice(User user, string deviceId, string deviceName);
    }
}