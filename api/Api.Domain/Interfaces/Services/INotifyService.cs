using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Services
{
    public interface INotifyService
    {
        // ToDo: implementation
        Task NewPendingMember(User user, Party party);
    }
}