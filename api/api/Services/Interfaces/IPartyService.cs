using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Services.Interfaces
{
    public interface IPartyService
    {
        Task<List<Party>> GetAll();
        Task<Party> Find(string id);
        Task<Party> FindByUser(string userId);
        Task<Party> Create(string ownerUserId, string name);
        Task RequestToJoin(string partyId, User user);
        Task Delete(string partyId);
        Task Leave(string userId);
        Task<bool> Exists(string id);

    }
}
