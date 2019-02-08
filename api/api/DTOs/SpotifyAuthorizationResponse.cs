namespace Api.DTOs
{
    public class SpotifyAuthorizationResponse
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public SpotifyAuthorizationResponse(string accessToken, int expiresIn)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
        }
    }
}