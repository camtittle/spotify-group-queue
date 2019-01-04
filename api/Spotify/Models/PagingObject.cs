using System.Collections.Generic;

namespace Spotify.Models
{
    public class PagingObject<T>
    {
        public List<T> Items;

        public int Limit;

        public string Next;

        public int Offset;

        public string Previous;

        public int Total;
    }
}