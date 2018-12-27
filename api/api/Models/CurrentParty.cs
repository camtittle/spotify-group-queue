using System.Collections.Generic;

namespace api.Models
{
    public class CurrentParty
    {
        public string Id;

        public string Name;

        public OtherUser Owner;

        public List<OtherUser> Members;

        public List<OtherUser> PendingMembers;
    }
}
