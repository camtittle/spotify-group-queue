using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Services
{
    public interface IPartyService
    {
        Task<Party> Create(User owner, string name);
        Task Delete(Party party);
        Task<SpotifyDevice> UpdateDevice(Party party, string deviceId, string deviceName);
    }
}
