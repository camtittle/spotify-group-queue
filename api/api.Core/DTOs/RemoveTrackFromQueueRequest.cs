using Microsoft.Build.Framework;

namespace api.Hubs.Models
{
    public class RemoveTrackFromQueueRequest
    {
        [Required]
        public string QueueItemId { get; set; }
    }
}