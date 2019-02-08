using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Services
{
    public interface IRealTimeService
    {
        Task NotifyOwnerOfPendingMember(User user, Party party);
        Task SendPlaybackStatusUpdate(Party party, User[] exemptUsers = null);
        Task SendPartyStatusUpdate(Party party);
    }
}