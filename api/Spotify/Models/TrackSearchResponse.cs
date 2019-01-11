namespace Spotify.Models
{
    public class TrackSearchResponse
    {
        public PagingObject<Track> Tracks { get; set; }
    }
}