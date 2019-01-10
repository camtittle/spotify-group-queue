using System.Threading.Tasks;

namespace Spotify.Interfaces
{
    public interface ISpotifyTokenManager
    {
        Task<string> GetAccessToken();
        string GetCredentialsBase64();
    }
}