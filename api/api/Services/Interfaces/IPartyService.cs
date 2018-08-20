using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Services.Interfaces
{
    public interface IPartyService
    {
        Task<Party> Find(string id);
        Task<Party> FindByUser(string userId);
        Task<Party> Create(string ownerUserId, string name);
        Task RequestToJoin(Party party, User user);
        Task Delete(string partyId);

    }
}
