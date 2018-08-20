using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public Party CurrentParty { get; set; }

        public bool RequestPending { get; set; }

        public bool Admin { get; set; }

        public bool Owner { get; set; }

    }
}
