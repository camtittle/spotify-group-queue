using System.ComponentModel.DataAnnotations;

namespace Api.Domain.DTOs
{
    public class RemoveTrackFromQueueRequest
    {
        [Required]
        public string QueueItemId { get; set; }
    }
}