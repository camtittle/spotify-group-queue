using System;

namespace Spotify.Exceptions
{
    public class SpotifyException : Exception
    {
        public SpotifyException(string message) : base(message)
        {
        }

        public SpotifyException(string message, Exception innereException) : base(message, innereException)
        {
        }
    }
}