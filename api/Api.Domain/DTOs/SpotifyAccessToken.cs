namespace Api.Domain.DTOs
{
    public class SpotifyAccessToken
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public SpotifyAccessToken(string token, int expiresIn)
        {
            AccessToken = token;
            ExpiresIn = expiresIn;
        }
    }
}
