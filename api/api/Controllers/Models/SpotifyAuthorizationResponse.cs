using Spotify.Models;

namespace api.Controllers.Models
{
    public class SpotifyAuthorizationResponse
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public string RefreshToken { get; set; }

        public SpotifyAuthorizationResponse(AuthorizationCodeTokenResponse response)
        {
            AccessToken = response.AccessToken;
            ExpiresIn = response.ExpiresIn;
            RefreshToken = response.RefreshToken;
        }
    }
}
