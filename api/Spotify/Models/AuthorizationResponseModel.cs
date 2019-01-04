using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Spotify
{
    public class AuthorizationResponseModel
    {
        [JsonProperty("access_token")]
        public string AccessToken;

        [JsonProperty("token_type")]
        public string TokenType;

        [JsonProperty("expires_in")]
        public long ExpiresIn;
    }
}