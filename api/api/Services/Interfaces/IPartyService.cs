using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers.Models;
using api.Hubs.Models;
using api.Models;

namespace api.Services.Interfaces
{
    public interface IPartyService
    {
        Task<List<Party>> GetAll();
        Task<Party> Find(string id);
        Task<Party> Create(User owner, string name);
        Task RequestToJoin(Party party, User user);
        Task Delete(Party party);
        Task Leave(User user);
        Task AddPendingMember(Party party, User user);
        Task RemovePendingMember(Party party, User user);
        Task<CurrentParty> GetCurrentParty(Party party);
        Task<Party> LoadFull(Party party);
        Task<QueueItem> AddQueueItem(User user, AddTrackToQueueRequest request);
    }
}
