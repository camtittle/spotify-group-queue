using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Spotify.Models
{
    public class ClientCredentialsTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken;

        [JsonProperty("token_type")]
        public string TokenType;

        [JsonProperty("expires_in")]
        public long ExpiresIn;
    }
}