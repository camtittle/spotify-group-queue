using System.Collections.Generic;

namespace Spotify.Models
{
    public class PagingObject<T>
    {
        public List<T> Items { get; set; }

        public int Limit { get; set; }

        public string Next { get; set; }

        public int Offset { get; set; }

        public string Previous { get; set; }

        public int Total { get; set; }
    }
}