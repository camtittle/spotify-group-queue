namespace Api.Domain.Settings
{
    public class SpotifySettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TokenUri { get; set; }
        public string RedirectUri { get; set; }
        public string BaseApiUri { get; set; }
    }
}
