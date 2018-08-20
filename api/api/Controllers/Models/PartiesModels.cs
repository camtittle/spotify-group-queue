using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers.Models
{
    public class CreatePartyModel
    {
        [Required]
        [StringLength(40)]
        public string Name;
    }
}
