using System.ComponentModel.DataAnnotations;

namespace api.Controllers.Models
{
    public class CreatePartyRequest
    {
        [Required]
        [StringLength(40)]
        public string Name { get; set; }
    }

    public class JoinPartyRequest
    {
        [Required]
        public string PartyId { get; set; }
    }

    public class GetPartiesResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }

        public int MemberCount { get; set; }
    }
}
