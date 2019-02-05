using System;

namespace Api.Business.Exceptions
{
    public class SpotifyPlaybackException : Exception
    {
        public SpotifyPlaybackException(string message) : base(message)
        {

        }
    }
}