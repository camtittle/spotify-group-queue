using System;

namespace api.Exceptions
{
    public class SpotifyPlaybackException : Exception
    {
        public SpotifyPlaybackException(string message) : base(message)
        {

        }
    }
}