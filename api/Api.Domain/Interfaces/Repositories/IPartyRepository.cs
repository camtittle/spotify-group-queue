using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Repositories
{
    public interface IPartyRepository
    {
        Task<Party> Add(Party party);
        Task<List<Party>> GetAll();
        Task<Party> Get(string id);
        Task<Party> GetByName(string name);
        Task<bool> Exists(string id);
        Task Delete(Party party);
        Task<Party> GetWithAllProperties(Party party);
        Task Update(Party party);
    }
}