using System;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.Entities
{
    [Owned]
    public class Track
    {
        public string Uri { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public int DurationMillis { get; set; }

        public DateTime ExpectedFinishTime { get; set; }
    }
}