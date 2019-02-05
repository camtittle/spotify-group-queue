using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Services
{
    public interface IMembershipService
    {
        Task RequestToJoin(Party party, User user);
        Task Leave(User user);
        Task AddPendingMember(Party party, User user);
        Task RemovePendingMember(Party party, User user);

    }
}