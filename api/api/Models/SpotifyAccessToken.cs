using Spotify.Models;

namespace api.Models
{
    public class SpotifyAccessToken
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public SpotifyAccessToken(AuthorizationCodeTokenResponse response)
        {
            AccessToken = response.AccessToken;
            ExpiresIn = response.ExpiresIn;
        }

        public SpotifyAccessToken(string token, int expiresIn)
        {
            AccessToken = token;
            ExpiresIn = expiresIn;
        }
    }
}
