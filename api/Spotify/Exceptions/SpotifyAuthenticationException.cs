using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Spotify
{
    class SpotifyAuthenticationException : Exception
    {
        public SpotifyAuthenticationException(string message) : base(message)
        {

        }

        public SpotifyAuthenticationException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
