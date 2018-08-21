using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers.Models
{
    public class TokenRequest
    {
        [Required]
        public string Username;

        [Required]
        public string DeveloperPassword;
    }

    public class RegisterRequest
    {
        [Required]
        [StringLength(30)]
        public string Username;
    }
}
