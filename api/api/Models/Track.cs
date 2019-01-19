using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace api.Models
{
    [Owned]
    public class Track
    {
        public string Uri { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public int DurationMillis { get; set; }
    }
}