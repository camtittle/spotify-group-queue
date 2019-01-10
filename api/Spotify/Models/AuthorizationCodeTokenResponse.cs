using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Spotify.Models
{
    public class AuthorizationCodeTokenResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; }

        [JsonProperty("token_type")] public string TokenType { get; set; }

        public string Scope { get; set; }

        [JsonProperty("expires_in")] public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")] public string RefreshToken { get; set; }
    }
}